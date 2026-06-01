/* ===================================================================
   internationalLicenseApplications.js  —  List & Filter International Licenses
=================================================================== */

const API_BASE = "https://localhost:7223/api/InternationalApplication/all";

/* ── State ─────────────────────────────────────────────────────── */
let allRecords  = [];
let filtered    = [];
let currentPage = 1;
let totalPages  = 1;
const PAGE_SIZE = 10;


document.getElementById("btnFirst").addEventListener("click", () => goToPage(1));
document.getElementById("btnPrev").addEventListener("click",  () => goToPage(currentPage - 1));
document.getElementById("btnNext").addEventListener("click",  () => goToPage(currentPage + 1));
document.getElementById("btnLast").addEventListener("click",  () => goToPage(totalPages));


function formatDate(isoString) {
  if (!isoString) return "—";
  return new Date(isoString).toLocaleDateString("en-GB", {
    day: "2-digit", month: "2-digit", year: "numeric",
  });
}

async function fetchAll() {
  const response = await fetch(API_BASE);
  if (!response.ok) throw new Error(`${response.status} ${response.statusText}`);
  return await response.json();
}

let LicenseID = 0;
function renderTable() {
  const tbody = document.getElementById("intlLicenseTableBody");
  const start = (currentPage - 1) * PAGE_SIZE;
  const page  = filtered.slice(start, start + PAGE_SIZE);

  if (page.length === 0) {
    tbody.innerHTML =
      '<tr><td colspan="8" style="text-align:center;color:var(--muted);padding:32px;">No records found.</td></tr>';
    return;
  }

  tbody.innerHTML = page.map(function(r) {
    var isActive   = r.isActive === true || r.isActive === 1;
    var badgeClass = isActive ? "badge-active"  : "badge-inactive";
    var badgeLabel = isActive ? "Active"          : "Inactive";
    LicenseID = r.licenseID;
    return '<tr>' +
      '<td>' + (r.internationalLicenseID ?? "—") + '</td>' +
      '<td>' + (r.applicationID          ?? "—") + '</td>' +
      '<td>' + (r.driverID               ?? "—") + '</td>' +
      '<td>' + (r.licenseID              ?? "—") + '</td>' +
      '<td>' + formatDate(r.issueDate)            + '</td>' +
      '<td>' + formatDate(r.expirationDate)       + '</td>' +
      '<td><span class="badge ' + badgeClass + '"><span class="badge-dot"></span>' + badgeLabel + '</span></td>' +
      '<td><div class="options">' +

        '<button class="btn-info" title="Show Person Details" onclick="showPersonDetails()">' +
          '<svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24"' +
          ' fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">' +
          '<circle cx="12" cy="8" r="4"/>' +
          '<path d="M4 20c0-4 3.6-7 8-7s8 3 8 7"/>' +
          '</svg>' +
        '</button>' +

        '<button class="btn-edit" title="Show License Details" onclick="showLicenseDetails(' + r.internationalLicenseID + ')">' +
          '<svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24"' +
          ' fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">' +
          '<rect x="2" y="5" width="20" height="14" rx="2"/>' +
          '<path d="M2 10h20"/>' +
          '</svg>' +
        '</button>' +

        '<button class="btn-process" title="Show Person License History" onclick="showLicenseHistory()">' +
          '<svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24"' +
          ' fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">' +
          '<path d="M3 3v5h5"/>' +
          '<path d="M3.05 13A9 9 0 1 0 6 5.3L3 8"/>' +
          '<path d="M12 7v5l4 2"/>' +
          '</svg>' +
        '</button>' +

      '</div></td>' +
    '</tr>';
  }).join("");
}

/* ── Pagination ────────────────────────────────────────────────── */
function goToPage(page) {
    if (page < 1 || page > totalPages) return;
    renderPage(page);
    renderPagination();
}

function renderPagination() {
  totalPages  = Math.max(1, Math.ceil(filtered.length / PAGE_SIZE));
  currentPage = Math.min(currentPage, totalPages);

  document.getElementById("pageInfo").textContent   = "Page " + currentPage + " of " + totalPages;
  document.getElementById("totalCount").textContent = "Total: " + filtered.length + " record" + (filtered.length !== 1 ? "s" : "");

  document.getElementById("btnFirst").disabled = currentPage === 1;
  document.getElementById("btnPrev").disabled  = currentPage === 1;
  document.getElementById("btnNext").disabled  = currentPage === totalPages;
  document.getElementById("btnLast").disabled  = currentPage === totalPages;

  var container = document.getElementById("pgNumbers");
  container.innerHTML = "";

  var delta = 2;
  var range = [];
  for (var i = Math.max(1, currentPage - delta); i <= Math.min(totalPages, currentPage + delta); i++) {
    range.push(i);
  }

  if (range[0] > 1) {
    container.innerHTML += '<button class="pg-btn" onclick="goToPage(1)">1</button>';
    if (range[0] > 2) container.innerHTML += '<span class="pg-ellipsis">…</span>';
  }

  range.forEach(function(n) {
    container.innerHTML += '<button class="pg-btn ' + (n === currentPage ? "active" : "") + '" onclick="goToPage(' + n + ')">' + n + '</button>';
  });

  if (range[range.length - 1] < totalPages) {
    if (range[range.length - 1] < totalPages - 1) container.innerHTML += '<span class="pg-ellipsis">…</span>';
    container.innerHTML += '<button class="pg-btn" onclick="goToPage(' + totalPages + ')">' + totalPages + '</button>';
  }
}

window.goToPage = function(page) {
  page = Math.max(1, Math.min(page, totalPages));
  if (page === currentPage) return;
  currentPage = page;
  renderTable();
  renderPagination();
};

/* ── Filter ────────────────────────────────────────────────────── */
function applyFilter() {
  var filterBy = document.getElementById("filterBy").value;
  var input    = document.getElementById("filterInput").value.trim().toLowerCase();
  var active   = document.getElementById("isActiveSelect").value;

  if (!filterBy) {
    filtered = allRecords.slice();
  } else if (filterBy === "IsActive") {
    filtered = active === ""
      ? allRecords.slice()
      : allRecords.filter(function(r) { return String(r.isActive) === active; });
  } else {
    if (!input) {
      filtered = allRecords.slice();
    } else {
      var keyMap = {
        InternationalLicenseID : "internationalLicenseID",
        ApplicationID          : "applicationID",
        DriverID               : "driverID",
        LocalLicenseID         : "licenseID",
      };
      var key = keyMap[filterBy];
      filtered = allRecords.filter(function(r) {
        return String(r[key] != null ? r[key] : "").toLowerCase().includes(input);
      });
    }
  }

  currentPage = 1;
  renderTable();
  renderPagination();
}

/* ── Filter UI controls ─────────────────────────────────────────── */
function setupFilterControls() {
  var filterBy       = document.getElementById("filterBy");
  var filterInput    = document.getElementById("filterInput");
  var isActiveSelect = document.getElementById("isActiveSelect");

  filterBy.addEventListener("change", function() {
    var val = filterBy.value;
    filterInput.style.display    = (val && val !== "IsActive") ? "block" : "none";
    isActiveSelect.style.display = val === "IsActive" ? "inline-block" : "none";
    filterInput.value    = "";
    isActiveSelect.value = "";
    applyFilter();
  });

  filterInput.addEventListener("input", applyFilter);
  isActiveSelect.addEventListener("change", applyFilter);
}

/* ── Navigation ─────────────────────────────────────────────────── */
window.showPersonDetails = function() {
  window.location.href = "/Sections/People/personInfo.html?id=" + PersonID;
};

window.showLicenseDetails = function(internationalLicenseID) {
  window.location.href = "/Sections/Applications/InternationalApplication/InternationalApplicationInfo.html?InternationID=" + internationalLicenseID;
};

window.showLicenseHistory = function(LocalApplicationID) {
  window.location.href = "/Sections/Applications/LocalApplication/LicenseHistory.html?id=" + PersonID;
};


let PersonID = 0;
/* ── Init ──────────────────────────────────────────────────────── */
document.addEventListener("DOMContentLoaded", async function() {
  setupFilterControls();
  
  try {
    allRecords = await fetchAll();
    filtered   = allRecords.slice();
    renderTable();
    PersonID = await GetPersonIDbyLicenseID(LicenseID);
    renderPagination();
  } catch (err) {
    console.error("Failed to load international license applications:", err);
    document.getElementById("intlLicenseTableBody").innerHTML =
      '<tr><td colspan="8" style="text-align:center;color:#a32d2d;padding:32px;">Failed to load data. ' + err.message + '</td></tr>';
  }
});

async function GetPersonIDbyLicenseID(LicenseID)
{
  try
  {
    const response = await fetch("https://localhost:7223/api/InternationalApplication/GetPersonIDByLicenseID?LicenseID=" + LicenseID);
    if (!response.ok) throw new Error(`${response.status} ${response.statusText}`);
    const data = await response.json();
    return data.personID;
  }
  catch (err) {
    console.error("Failed to fetch person ID:", err);
    throw err;
  }
}