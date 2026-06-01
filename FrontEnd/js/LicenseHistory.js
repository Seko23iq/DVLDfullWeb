import {formatDate} from "./Global.js";

let allRecords = [];

function getBadge(isActive) {
    const badgeClass = isActive ? "badge-active" : "badge-inactive";
    const badgeLabel = isActive ? "Active" : "Inactive";
    return { badgeClass, badgeLabel };
}

function getPersonIDFromURL() {
    const params = new URLSearchParams(window.location.search);
    return params.get("id");
}

const PersonID = getPersonIDFromURL();
async function LocalLocalLicenseHistoryRecords() {
    try {
        const res = await fetch(`https://localhost:7223/api/LocalDrivingApplication/AllLocalLicenseHistoryRecordsByPersonID/${PersonID}`);
        if (!res.ok) throw new Error("Failed to fetch records");

        const data = await res.json();

        allRecords = data;
        renderLocalLocalLicenseHistoryPage(data);
    } catch (err) {
        console.error(err);
    }
}
function renderLocalLocalLicenseHistoryPage(Records) {
    const tableBody = document.getElementById("localLicensesBody");

    tableBody.innerHTML = "";

    if (Records.length === 0) {
        tableBody.innerHTML = `
            <tr><td colspan="7" style="text-align:center;color:var(--muted);padding:32px;">
                No records found.
            </td></tr>`;
        return;
    }

    Records.forEach((record, index) => {
        const { badgeClass, badgeLabel } = getBadge(record.isActive);

        tableBody.innerHTML += `
            <tr>
                <td>${record.licenseID ?? "—"}</td>
                <td>${record.applicationID ?? "—"}</td>
                <td>${record.licenseClassName ?? "—"}</td>
                <td>${formatDate(record.issueDate)}</td>
                <td>${formatDate(record.expirationDate)}</td>
                <td>
                    <span class="badge ${badgeClass}">
                        <span class="badge-dot"></span>
                        ${badgeLabel}
                    </span>
                </td>
                <td>
                <button class="btn-license-info" id="LicenseInfo" onclick="showLicenseInfo(${record.licenseID})">
                    <svg xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                        <rect x="2" y="5" width="20" height="14" rx="2"/>
                        <circle cx="8" cy="12" r="2"/>
                        <path d="M14 9h4M14 12h4M14 15h2"/>
                    </svg>
                    License Info
                </button>
                </td>
            </tr>`;
    });
}

window.showLicenseInfo = function(LicenseID) {
    window.location.href = `/Sections/Applications/LocalApplication/DriverLicenseInfo.html?LicenseID=${LicenseID}`;
}
window.showInternationalLicenseInfo = function(InternationID) {
    window.location.href = `/Sections/Applications/InternationalApplication/InternationalApplicationInfo.html?InternationID=${InternationID}`;
}
async function InternationalLicenseHistoryRecords() {
    try {
        const res = await fetch(`https://localhost:7223/api/InternationalApplication/GetAllInternationalLicenseHistroyRecordsByPersonID/${PersonID}`);
        if (!res.ok) throw new Error("Failed to fetch records");

        const data = await res.json();

        allRecords = data;
        renderInternationalPage(data);
    } catch (err) {
        console.error(err);
    }
}
function renderInternationalPage(Records) {
    const tableBody = document.getElementById("internationalLicensesBody");

    tableBody.innerHTML = "";

    if (Records.length === 0) {
        tableBody.innerHTML = `
            <tr><td colspan="7" style="text-align:center;color:var(--muted);padding:32px;">
                No records found.
            </td></tr>`;
        return;
    }

    Records.forEach((record, index) => {
        const { badgeClass, badgeLabel } = getBadge(record.isActive);

        tableBody.innerHTML += `
            <tr>
                <td>${record.internationalLicenseID ?? "—"}</td>
                <td>${record.applicationID ?? "—"}</td>
                <td>${record.licenseClassName ?? "—"}</td>
                <td>${formatDate(record.issueDate)}</td>
                <td>${formatDate(record.expirationDate)}</td>
                <td>
                    <span class="badge ${badgeClass}">
                        <span class="badge-dot"></span>
                        ${badgeLabel}
                    </span>
                </td>
                <td>
                <button class="btn-license-info" onclick="showInternationalLicenseInfo(${record.internationalLicenseID})">
                    <svg xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                        <rect x="2" y="5" width="20" height="14" rx="2"/>
                        <circle cx="8" cy="12" r="2"/>
                        <path d="M14 9h4M14 12h4M14 15h2"/>
                    </svg>
                    License Info
                </button>
                </td>
            </tr>`;
    });
}

window.onload = () => {
    LocalLocalLicenseHistoryRecords();
    InternationalLicenseHistoryRecords();
};