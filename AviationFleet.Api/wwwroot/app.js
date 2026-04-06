const api = {
  async request(method, path, body) {
    const opts = { method, headers: {} };
    if (body !== undefined) {
      opts.headers["Content-Type"] = "application/json";
      opts.body = JSON.stringify(body);
    }
    const res = await fetch(path, opts);
    const text = await res.text();
    let data = null;
    if (text) {
      try {
        data = JSON.parse(text);
      } catch {
        data = text;
      }
    }
    if (!res.ok) {
      const msg =
        data && typeof data === "object" && data.error
          ? data.error
          : typeof data === "string"
            ? data
            : res.statusText;
      throw new Error(msg || `Request failed (${res.status})`);
    }
    return data;
  },
  get: (p) => api.request("GET", p),
  post: (p, b) => api.request("POST", p, b),
  put: (p, b) => api.request("PUT", p, b),
  del: (p) => api.request("DELETE", p),
};

const CERT_LABELS = ["Avionics", "Engine", "Hydraulics"];
const TICKET_STATUS = ["Open", "In progress", "Completed"];
const LOCATION = ["On the ground", "In the air"];
const OPS = ["Ready", "Maintenance required", "In maintenance", "Unavailable (overhaul)"];

let aircraftCache = [];

function toast(message, type = "ok") {
  const host = document.getElementById("toasts");
  const el = document.createElement("div");
  el.className = `toast toast--${type === "err" ? "err" : "ok"}`;
  el.textContent = message;
  host.appendChild(el);
  setTimeout(() => {
    el.style.opacity = "0";
    el.style.transition = "opacity 0.3s";
    setTimeout(() => el.remove(), 320);
  }, 4200);
}

function showThresholdAlert(message) {
  const dock = document.getElementById("alert-dock");
  const el = document.createElement("div");
  el.className = "alert-card";
  el.innerHTML = `<strong>Maintenance alert</strong><p>${escapeHtml(message)}</p><button type="button" class="btn btn-sm btn-ghost dismiss">Dismiss</button>`;
  el.querySelector(".dismiss").addEventListener("click", () => el.remove());
  dock.appendChild(el);
  setTimeout(() => {
    if (el.parentNode) el.remove();
  }, 45000);
}

function escapeHtml(s) {
  return String(s)
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;");
}

function fmtNum(n) {
  const x = Number(n);
  if (Number.isFinite(x)) return x % 1 === 0 ? String(x) : x.toFixed(1);
  return String(n);
}

function airframeRemaining(a) {
  return a.maintenanceThresholdHours - (a.totalFlightHours - a.hoursAtLastMaintenance);
}

async function loadFleetSettingsForm() {
  try {
    const s = await api.get("/api/settings/fleet");
    document.querySelector('#form-settings [name="maxJobs"]').value = s.maxMechanicActiveJobs;
  } catch {
    /* ignore */
  }
}

async function loadAircraft() {
  aircraftCache = await api.get("/api/aircraft");
  renderStatusTable();
  renderAircraftCards();
  fillTicketAircraftSelect();
}

function renderStatusTable() {
  const tbody = document.querySelector("#table-status tbody");
  const empty = document.getElementById("empty-status");
  tbody.innerHTML = "";
  if (!aircraftCache.length) {
    empty.hidden = false;
    return;
  }
  empty.hidden = true;
  for (const a of aircraftCache) {
    const rem = airframeRemaining(a);
    const tr = document.createElement("tr");
    tr.innerHTML = `
      <td class="mono"><strong>${escapeHtml(a.tailNumber)}</strong></td>
      <td>${escapeHtml(LOCATION[a.locationState] ?? a.locationState)}</td>
      <td>${escapeHtml(OPS[a.operationalStatus] ?? a.operationalStatus)}</td>
      <td class="mono">${fmtNum(a.totalFlightHours)}</td>
      <td class="mono">${fmtNum(rem)}</td>
      <td class="status-actions"></td>`;
    const cell = tr.querySelector(".status-actions");
    cell.appendChild(mkBtn("Hours", () => quickUpdateHours(a)));
    cell.appendChild(mkBtn("Location", () => quickCycleLocation(a)));
    cell.appendChild(mkBtn("Ops status", () => quickOperational(a)));
    cell.appendChild(mkBtn("Confirm ready", () => confirmReady(a)));
    cell.appendChild(mkBtn("Remove", () => deleteAircraft(a), "btn-sm btn-danger"));
    tbody.appendChild(tr);
  }
}

function renderAircraftCards() {
  const host = document.getElementById("aircraft-detail-cards");
  host.innerHTML = "";
  if (!aircraftCache.length) {
    host.innerHTML = '<p class="empty">No aircraft.</p>';
    return;
  }
  for (const a of aircraftCache) {
    const card = document.createElement("div");
    card.className = "aircraft-card";
    const parts = (a.parts || [])
      .map((p) => {
        const pid = p.id ?? "";
        const pname = p.partName ?? String(p.partType);
        return `<tr>
          <td class="mono part-id" title="${escapeHtml(pid)}">${escapeHtml(pid)}</td>
          <td>${escapeHtml(pname)}</td>
          <td class="mono">${fmtNum(p.hoursSinceLastMaintenance)}</td>
          <td class="mono">${fmtNum(p.maintenanceThresholdHours)}</td>
        </tr>`;
      })
      .join("");
    card.innerHTML = `
      <div class="card-head">
        <div>
          <h3>${escapeHtml(a.tailNumber)}</h3>
          <p class="meta">${escapeHtml(LOCATION[a.locationState])} · ${escapeHtml(OPS[a.operationalStatus])}</p>
        </div>
        <button type="button" class="btn btn-sm btn-danger" data-remove-aircraft="${escapeHtml(a.id)}">Remove aircraft</button>
      </div>
      <table class="data-table compact inner">
        <thead><tr><th>Part ID</th><th>Component</th><th>Hrs since svc</th><th>Threshold (hrs)</th></tr></thead>
        <tbody>${parts}</tbody>
      </table>`;
    card.querySelector("[data-remove-aircraft]")?.addEventListener("click", () => deleteAircraft(a));
    host.appendChild(card);
  }
}

function mkBtn(label, onClick, cls = "btn-sm btn-ghost") {
  const b = document.createElement("button");
  b.type = "button";
  b.className = `btn ${cls}`;
  b.textContent = label;
  b.addEventListener("click", onClick);
  return b;
}

async function loadMechanics() {
  const rows = await api.get("/api/mechanics");
  const tbody = document.querySelector("#table-mechanics tbody");
  const empty = document.getElementById("empty-mechanics");
  tbody.innerHTML = "";
  if (!rows.length) {
    empty.hidden = false;
    return;
  }
  empty.hidden = true;
  for (const m of rows) {
    const tr = document.createElement("tr");
    tr.innerHTML = `
      <td>${escapeHtml(m.name)}</td>
      <td>${escapeHtml(CERT_LABELS[m.certification] ?? m.certification)}</td>
      <td>${m.isAvailable ? '<span class="badge badge--ok">Yes</span>' : '<span class="badge badge--stop">No</span>'}</td>
      <td class="mono">${m.currentWorkload}</td>
      <td class="nowrap"></td>`;
    const cell = tr.querySelector("td:last-child");
    cell.appendChild(mkBtn("Remove", () => deleteMechanic(m), "btn-sm btn-danger"));
    tbody.appendChild(tr);
  }
}

async function deleteAircraft(a) {
  if (!confirm(`Remove ${a.tailNumber} from the fleet? This deletes its tickets and components.`)) return;
  try {
    await api.del(`/api/aircraft/${a.id}`);
    toast(`${a.tailNumber} removed.`);
    await refreshAll();
  } catch (e) {
    toast(e.message, "err");
  }
}

async function deleteMechanic(m) {
  if (!confirm(`Remove ${m.name} from the crew? Open/in-progress tickets will be unassigned.`)) return;
  try {
    await api.del(`/api/mechanics/${m.id}`);
    toast(`${m.name} removed.`);
    await refreshAll();
  } catch (e) {
    toast(e.message, "err");
  }
}

async function loadWarnings() {
  const rows = await api.get("/api/maintenance/warnings");
  const tbody = document.querySelector("#table-warnings tbody");
  const empty = document.getElementById("empty-warnings");
  tbody.innerHTML = "";
  if (!rows.length) {
    empty.hidden = false;
    return;
  }
  empty.hidden = true;
  for (const w of rows) {
    const tr = document.createElement("tr");
    tr.innerHTML = `
      <td class="mono"><strong>${escapeHtml(w.tailNumber)}</strong></td>
      <td class="mono">${fmtNum(w.hoursRemainingUntilThreshold)}</td>
      <td>${escapeHtml(w.message)}</td>`;
    tbody.appendChild(tr);
  }
}

function fillTicketAircraftSelect() {
  const sel = document.getElementById("ticket-aircraft-select");
  sel.innerHTML = "";
  if (!aircraftCache.length) {
    const o = document.createElement("option");
    o.value = "";
    o.textContent = "Add an aircraft first";
    sel.appendChild(o);
    sel.disabled = true;
    return;
  }
  sel.disabled = false;
  for (const a of aircraftCache) {
    const o = document.createElement("option");
    o.value = a.id;
    o.textContent = a.tailNumber;
    sel.appendChild(o);
  }
}

async function loadAllTicketsModal() {
  const rows = await api.get("/api/maintenance/tickets");
  const tbody = document.querySelector("#table-all-tickets tbody");
  const empty = document.getElementById("empty-all-tickets");
  tbody.innerHTML = "";
  if (!rows.length) {
    empty.hidden = false;
    return;
  }
  empty.hidden = true;
  for (const t of rows) {
    const tr = document.createElement("tr");
    tr.innerHTML = `
      <td class="mono">${escapeHtml(t.tailNumber)}</td>
      <td class="mono nowrap">${escapeHtml(t.dateCreatedDisplay)}</td>
      <td>${escapeHtml(TICKET_STATUS[t.status] ?? t.status)}</td>
      <td>${escapeHtml(t.mechanicName ?? "—")}</td>
      <td>${escapeHtml(CERT_LABELS[t.requiredCertification] ?? t.requiredCertification)}</td>
      <td>${t.isAutoGenerated ? "Yes" : "No"}</td>
      <td>${escapeHtml(t.relatedPartLabel ?? "—")}</td>
      <td class="issue">${escapeHtml(t.issueDescription)}</td>`;
    tbody.appendChild(tr);
  }
}

function wireTabs() {
  const tabs = document.querySelectorAll(".tab");
  const panels = {
    status: document.getElementById("panel-status"),
    fleet: document.getElementById("panel-fleet"),
    crew: document.getElementById("panel-crew"),
    tickets: document.getElementById("panel-tickets"),
    predict: document.getElementById("panel-predict"),
  };
  tabs.forEach((tab) => {
    tab.addEventListener("click", () => {
      const key = tab.dataset.tab;
      tabs.forEach((t) => t.classList.toggle("is-active", t === tab));
      Object.entries(panels).forEach(([k, el]) => {
        const on = k === key;
        el.classList.toggle("is-visible", on);
        el.hidden = !on;
        if (k === "tickets") fillTicketAircraftSelect();
      });
    });
  });
}

document.querySelectorAll("[data-close-dialog]").forEach((b) => {
  b.addEventListener("click", () => b.closest("dialog")?.close());
});

document.getElementById("btn-open-aircraft").addEventListener("click", () => {
  document.getElementById("form-aircraft").reset();
  document.getElementById("modal-aircraft").showModal();
});

document.getElementById("btn-open-mechanic").addEventListener("click", () => {
  const f = document.getElementById("form-mechanic");
  f.reset();
  f.isAvailable.checked = true;
  document.getElementById("modal-mechanic").showModal();
});

document.getElementById("btn-settings").addEventListener("click", async () => {
  await loadFleetSettingsForm();
  document.getElementById("modal-settings").showModal();
});

document.getElementById("btn-all-tickets").addEventListener("click", async () => {
  try {
    await loadAllTicketsModal();
    document.getElementById("modal-tickets-list").showModal();
  } catch (e) {
    toast(e.message, "err");
  }
});

document.getElementById("form-settings").addEventListener("submit", async (e) => {
  e.preventDefault();
  const max = Number(e.target.maxJobs.value);
  try {
    await api.put("/api/settings/fleet", { maxMechanicActiveJobs: max });
    toast("Fleet settings saved.");
    document.getElementById("modal-settings").close();
  } catch (err) {
    toast(err.message, "err");
  }
});

async function quickUpdateHours(a) {
  const v = prompt(`New total flight hours for ${a.tailNumber}?`, String(a.totalFlightHours));
  if (v === null) return;
  const n = parseFloat(v);
  if (!Number.isFinite(n) || n < 0) {
    toast("Invalid number.", "err");
    return;
  }
  try {
    const res = await api.put(`/api/aircraft/${a.id}/hours`, { totalFlightHours: n });
    if (res.thresholdExceeded && res.alertMessage) showThresholdAlert(res.alertMessage);
    if (res.autoTicketCreated) toast("Automatic maintenance ticket was created (on ground).");
    toast(`Updated ${a.tailNumber}.`);
    await refreshAll();
  } catch (e) {
    toast(e.message, "err");
  }
}

async function quickCycleLocation(a) {
  const next = a.locationState === 0 ? 1 : 0;
  try {
    await api.put(`/api/aircraft/${a.id}/location`, { locationState: next });
    toast(`Location updated for ${a.tailNumber}.`);
    await refreshAll();
  } catch (e) {
    toast(e.message, "err");
  }
}

async function quickOperational(a) {
  const msg = `Operational status for ${a.tailNumber}:\n0 Ready\n1 Maintenance required\n2 In maintenance\n3 Unavailable (major repair)`;
  const v = prompt(msg, String(a.operationalStatus));
  if (v === null) return;
  const s = parseInt(v, 10);
  if (![0, 1, 2, 3].includes(s)) {
    toast("Use 0–3.", "err");
    return;
  }
  try {
    await api.put(`/api/aircraft/${a.id}/operational-status`, { operationalStatus: s });
    toast(`Operational status updated.`);
    await refreshAll();
  } catch (e) {
    toast(e.message, "err");
  }
}

async function confirmReady(a) {
  if (!confirm(`Mark ${a.tailNumber} maintenance complete and ready to fly? Open tickets will be closed and counters reset.`))
    return;
  try {
    await api.post(`/api/aircraft/${a.id}/confirm-maintenance-complete`, {});
    toast(`${a.tailNumber} is ready.`);
    await refreshAll();
  } catch (e) {
    toast(e.message, "err");
  }
}

document.getElementById("form-aircraft").addEventListener("submit", async (e) => {
  e.preventDefault();
  const f = e.target;
  const body = {
    tailNumber: f.tailNumber.value.trim(),
    totalFlightHours: parseFloat(f.totalFlightHours.value),
    maintenanceThresholdHours: parseFloat(f.maintenanceThresholdHours.value),
    hoursAtLastMaintenance: parseFloat(f.hoursAtLastMaintenance.value),
    locationState: Number(f.locationState.value),
    operationalStatus: Number(f.operationalStatus.value),
  };
  try {
    await api.post("/api/aircraft", body);
    toast(`Aircraft ${body.tailNumber} added.`);
    document.getElementById("modal-aircraft").close();
    await refreshAll();
  } catch (err) {
    toast(err.message, "err");
  }
});

document.getElementById("form-mechanic").addEventListener("submit", async (e) => {
  e.preventDefault();
  const f = e.target;
  const body = {
    name: f.name.value.trim(),
    certification: Number(f.certification.value),
    isAvailable: f.isAvailable.checked,
  };
  try {
    await api.post("/api/mechanics", body);
    toast(`Mechanic ${body.name} added.`);
    document.getElementById("modal-mechanic").close();
    await refreshAll();
  } catch (err) {
    toast(err.message, "err");
  }
});

document.getElementById("form-ticket-inline").addEventListener("submit", async (e) => {
  e.preventDefault();
  const f = e.target;
  const aircraftId = f.aircraftId.value;
  if (!aircraftId) {
    toast("Select an aircraft.", "err");
    return;
  }
  const body = {
    aircraftId,
    issueDescription: f.issueDescription.value.trim(),
    requiredCertification: Number(f.requiredCertification.value),
  };
  try {
    await api.post("/api/maintenance/ticket", body);
    toast("Ticket created.");
    f.reset();
    fillTicketAircraftSelect();
    await refreshAll();
  } catch (err) {
    toast(err.message, "err");
  }
});

document.getElementById("btn-refresh-all").addEventListener("click", () => refreshAll());
document.getElementById("btn-run-predict").addEventListener("click", () =>
  loadWarnings().catch((e) => toast(e.message, "err")),
);

async function refreshAll() {
  try {
    await loadAircraft();
    await loadMechanics();
    await loadWarnings();
    fillTicketAircraftSelect();
  } catch (e) {
    toast(e.message, "err");
  }
}

wireTabs();
refreshAll();
