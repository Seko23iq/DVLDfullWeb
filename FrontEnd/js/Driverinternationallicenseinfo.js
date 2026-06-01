import {formatDate,showToast} from "./Global.js";
const API_BASE = "https://localhost:7223/api/InternationalApplication/info";

/* ── Enums ─────────────────────────────────────────────────────── */
const GENDER_MAP = {
  0: "Male",
  1: "Female",
};


function getUrlParam(name) {
  const params = new URLSearchParams(window.location.search);
  return params.get(name);
}

function buildStatusBadge(isActive) {
  return isActive
    ? `<span class="status-badge status-active">Active</span>`
    : `<span class="status-badge status-inactive">Inactive</span>`;
}

function renderInfo(data) {
  // Name
  document.querySelector(".Name").textContent = data.fullName ?? "—";

  // Int. License ID
  document.querySelector(".IntLicenseID").textContent =
    data.internationalLicenseID != null ? String(data.internationalLicenseID) : "—";

  // Application ID
  document.querySelector(".AppID").textContent =
    data.applicationID != null ? String(data.applicationID) : "—";

  // License ID
  document.querySelector(".LicenseID").textContent =
    data.licenseID != null ? String(data.licenseID) : "—";

  // Is Active badge
  document.querySelector(".IsActive").innerHTML = buildStatusBadge(data.isActive);

  // National No
  document.querySelector(".NationalNo").textContent = data.nationalNo ?? "—";

  // Date Of Birth
  document.querySelector(".DateOfBirth").textContent = formatDate(data.dateOfBirth);

  // Gender
  document.querySelector(".Gendor").textContent = GENDER_MAP[data.gendor] ?? "—";

  // Driver ID
  document.querySelector(".DriverID").textContent =
    data.driverID != null ? String(data.driverID) : "—";

  // Issue Date
  document.querySelector(".IssueDate").textContent = formatDate(data.issueDate);

  // Expiration Date
  document.querySelector(".ExpirationDate").textContent = formatDate(data.expirationDate);
}

function showLoading() {
  document.querySelectorAll(".licenseInfo div span:last-child").forEach((el) => {
    el.textContent  = "Loading...";
    el.style.color  = "var(--muted)";
  });
}

async function fetchInternationalLicenseInfo(InternationID) {
  const url = `${API_BASE}/${InternationID}`;

  const response = await fetch(url);

  if (!response.ok) {
    throw new Error(`Request failed: ${response.status} ${response.statusText}`);
  }

  return await response.json();
}

document.addEventListener("DOMContentLoaded",  () => {
  GetInternationInfo();
});

async function GetInternationInfo()
{
  const InternationID = getUrlParam("InternationID");

  if (!InternationID) {
    showToast("Missing InternationID in URL.", "error");
    return;
  }

  showLoading();

  try {
    const data = await fetchInternationalLicenseInfo(InternationID);
    renderInfo(data);
  } catch (err) {
    console.error("Failed to load international license info:", err);
    showToast(`Failed to load data. ${err.message}`, "error");
  }
}

const btnClose = document.querySelector(".btn-close");
btnClose.addEventListener("click", () => {
  history.back();
})