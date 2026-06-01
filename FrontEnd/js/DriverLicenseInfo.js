/* ===================================================================
   driverLicenseInfoScript.js  —  Fetch & Render Driver License Info
=================================================================== */

const API_BASE = "https://localhost:7223/api/LocalDrivingApplication/license-info";

/* ── Enums ─────────────────────────────────────────────────────── */
const GENDER_MAP = {
  0: "Male",
  1: "Female",
};
const ISSUE_REASON_MAP = {
  1: "New License",
  2: "Renewal",
  3: "Replacement for Damaged License",
  4: "Replacement for Lost License",
};

/* ── Helpers ───────────────────────────────────────────────────── */
function formatDate(isoString) {
  if (!isoString) return "—";
  const date = new Date(isoString);
  return date.toLocaleDateString("en-GB", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
  });
}

function getUrlParam(name) {
  const params = new URLSearchParams(window.location.search);
  return params.get(name);
}

function buildStatusBadge(isActive) {
  if (isActive) {
    return `<span class="status-badge status-active">Active</span>`;
  }
  return `<span class="status-badge status-inactive">Inactive</span>`;
}

function buildDetainedBadge(isDetained) {
  if (isDetained) {
    return `<span class="status-badge status-detained">Detained</span>`;
  }
  return `<span class="status-badge status-free">Not Detained</span>`;
}

/* ── Render ────────────────────────────────────────────────────── */
function renderLicenseInfo(data) {
  console.log(data);
  // Class banner
  document.querySelector(".licenseClass .Class").textContent =
    data.className ?? "—";

  // Name
  document.querySelector(".Name").textContent = data.fullName ?? "—";

  // License ID
  document.querySelector(".LicenseID").textContent =
    data.licenseID != null ? `${data.licenseID}` : "—";

  // isActive badge
  document.querySelector(".IsActive").innerHTML = buildStatusBadge(
    data.isActive
  );

  // National No
  document.querySelector(".NationalNo").textContent = data.nationalNo ?? "—";

  // Date Of Birth
  document.querySelector(".DateOfBirth").textContent = formatDate(
    data.dateOfBirth
  );

  // Gender
  document.querySelector(".Gendor").textContent =
    GENDER_MAP[data.gendor] ?? "—";

  // Driver ID
  document.querySelector(".DriverID").textContent =
    data.driverID != null ? `${String(data.driverID)}` : "—";

  // Issue Date
  document.querySelector(".IssueDate").textContent = formatDate(data.issueDate);

  // Expiration Date
  document.querySelector(".ExpirationDate").textContent = formatDate(
    data.expirationDate
  );

  // Issue Reason
  document.querySelector(".IssueReason").textContent =
    ISSUE_REASON_MAP[data.issueReason] ?? "—";

  // Is Detained — the JSON doesn't include isDetained, default to false
  document.querySelector(".IsDetained").innerHTML = buildDetainedBadge(
    data.isDetained ?? false
  );

  // Notes
  document.querySelector(".Notes").textContent =
    data.notes?.trim() || "No additional notes for this license.";
}

/* ── Loading & Error states ────────────────────────────────────── */
function showLoading() {
  document.querySelectorAll(".licenseInfo div span:last-child").forEach((el) => {
    el.textContent = "Loading...";
    el.style.color = "var(--muted)";
  });
  document.querySelector(".licenseClass .Class").textContent = "Loading...";
}

function showError(message) {
  document.querySelectorAll(".licenseInfo div span:last-child").forEach((el) => {
    el.textContent = "—";
    el.style.color = "var(--muted)";
  });
  document.querySelector(".licenseClass .Class").textContent = "Error";

  const notes = document.querySelector(".Notes");
  if (notes) {
    notes.textContent = message;
    notes.style.color = "#a32d2d";
  }
}

/* ── Fetch ─────────────────────────────────────────────────────── */
async function fetchLicenseInfo(LicenseID) {
  const url = `${API_BASE}?LicenseID=${LicenseID}`;

  const response = await fetch(url);

  if (!response.ok) {
    throw new Error(
      `Request failed: ${response.status} ${response.statusText}`
    );
  }

  return await response.json();
}

/* ── Init ──────────────────────────────────────────────────────── */
async function init() {
  const LicenseID = getUrlParam("LicenseID");

  if (!LicenseID) {
    showError("Missing LicenseID in URL.");
    return;
  }

  showLoading();

  try {
    const data = await fetchLicenseInfo(LicenseID);
    renderLicenseInfo(data);
  } catch (err) {
    console.error("Failed to load license info:", err);
    showError(`Failed to load data. ${err.message}`);
  }
}

document.addEventListener("DOMContentLoaded", init);