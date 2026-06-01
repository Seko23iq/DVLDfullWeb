import {GetPersonIDbyLicenseID} from "./Global.js";

function onFilterByChange() {
    const val = document.getElementById('filterBy').value;
    const textInput      = document.getElementById('filterInput');
    const releasedSelect = document.getElementById('isReleasedSelect');
    textInput.style.display      = 'none';
    releasedSelect.style.display = 'none';
    if (val === 'IsReleased') {
        releasedSelect.style.display = 'inline-block';
    } else if (val !== '') {
        textInput.style.display = 'inline-block';
    }
}
document.addEventListener('click', function (e) {
    // Close all open dropdowns first
    document.querySelectorAll('.options-wrapper.open').forEach(w => {
        if (!w.contains(e.target)) w.classList.remove('open');
    });
    // Toggle clicked one
    const btn = e.target.closest('.btn-options');
    if (btn) {
        const wrapper = btn.closest('.options-wrapper');
        wrapper.classList.toggle('open');
    }
});

const API_URL = 'https://localhost:7223/api/DetainReleaseLicense/GetAllDeatinedLicenses';

const PAGE_SIZE = 10;

let allRecords   = [];   
let filtered     = [];   
let currentPage  = 1;
let totalPages   = 1;


document.getElementById("btnFirst").addEventListener("click", () => goToPage(1));
document.getElementById("btnPrev").addEventListener("click",  () => goToPage(currentPage - 1));
document.getElementById("btnNext").addEventListener("click",  () => goToPage(currentPage + 1));
document.getElementById("btnLast").addEventListener("click",  () => goToPage(totalPages));


/* ── Boot ─────────────────────────────────────────────────────── */
document.addEventListener('DOMContentLoaded', async () => {
    await loadData();
    bindFilterEvents();
});

/* ── Fetch ────────────────────────────────────────────────────── */
async function loadData() {
    try {
        const res = await fetch(API_URL);
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        allRecords = await res.json();
        filtered   = [...allRecords];
        renderTable();
    } catch (err) {
        showError('Failed to load data: ' + err.message);
    }
}

/* ── Filter ───────────────────────────────────────────────────── */
function bindFilterEvents() {
    document.getElementById('filterInput')
        ?.addEventListener('input', applyFilter);

    document.getElementById('isReleasedSelect')
        ?.addEventListener('change', applyFilter);
}

function applyFilter() {
    const filterBy       = document.getElementById('filterBy').value;
    const textVal        = document.getElementById('filterInput').value.trim().toLowerCase();
    const releasedVal    = document.getElementById('isReleasedSelect').value;

    filtered = allRecords.filter(r => {
        switch (filterBy) {
            case 'DetainID':
                return String(r.detainID).includes(textVal);
            case 'IsReleased':
                if (releasedVal === '') return true;
                return r.isReleased === (releasedVal === 'true');
            case 'NationalNo':
                return r.nationalNo.toLowerCase().includes(textVal);
            case 'FullName':
                return r.fullName.toLowerCase().includes(textVal);
            case 'ReleaseAppID':
                return r.releaseApplicationID !== null &&
                       String(r.releaseApplicationID).includes(textVal);
            default:
                return true;
        }
    });

    currentPage = 1;
    renderTable();
}

/* ── Render ───────────────────────────────────────────────────── */
async function renderTable() {
    totalPages = Math.max(1, Math.ceil(filtered.length / PAGE_SIZE));
    currentPage = Math.min(currentPage, totalPages);

    const start = (currentPage - 1) * PAGE_SIZE;
    const page  = filtered.slice(start, start + PAGE_SIZE);

    const tbody = document.getElementById('detainTableBody');
    tbody.innerHTML = '';

    if (page.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="10" style="text-align:center; padding:32px; color:var(--muted); font-size:14px;">
                    No records found.
                </td>
            </tr>`;
    } else {
        const rows = await Promise.all(page.map(r => buildRow(r)));
        rows.forEach(row => tbody.appendChild(row));
    }

    updatePagination();
}

let PersonID = 0;
async function buildRow(r) {

    const tr = document.createElement('tr');
    PersonID = await GetPersonIDbyLicenseID(r.licenseID);
    const isReleasedBadge = r.isReleased
        ? `<span class="badge-released">Released</span>`
        : `<span class="badge-detained">Detained</span>`;

    const releaseDate = r.releaseDate
        ? `<span class="date-cell">${formatDate(r.releaseDate)}</span>`
        : `<span class="date-cell empty">—</span>`;

    const releaseAppID = r.releaseApplicationID !== null
        ? `<span class="app-id-cell">${r.releaseApplicationID}</span>`
        : `<span class="app-id-cell empty">—</span>`;

    tr.innerHTML = `
        <td>${r.detainID}</td>
        <td>${r.licenseID}</td>
        <td class="date-cell">${formatDate(r.detainDate)}</td>
        <td>${isReleasedBadge}</td>
        <td class="fees-cell">$${r.fineFees.toLocaleString()}</td>
        <td>${releaseDate}</td>
        <td>${r.nationalNo}</td>
        <td>${r.fullName}</td>
        <td>${releaseAppID}</td>
        <td>
            <div class="options-wrapper">
                <button class="btn-options">
                    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"
                        fill="none" stroke="currentColor" stroke-width="2"
                        stroke-linecap="round" stroke-linejoin="round">
                        <circle cx="12" cy="5"  r="1"/><circle cx="12" cy="12" r="1"/>
                        <circle cx="12" cy="19" r="1"/>
                    </svg>
                    Options
                </button>
                <div class="options-dropdown">
                    <a href="/Sections/People/PersonInfo.html?id=${PersonID}">
                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none"
                            stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                            <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/>
                            <circle cx="12" cy="7" r="4"/>
                        </svg>
                        Show Person Info
                    </a>
                    <a href="/Sections/Applications/LocalApplication/DriverLicenseInfo.html?LicenseID=${r.licenseID}">
                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none"
                            stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                            <rect x="2" y="5" width="20" height="14" rx="2"/>
                            <path d="M2 10h20"/>
                        </svg>
                        Show License Info
                    </a>
                    <a href="/Sections/Applications/LocalApplication/LicenseHistory.html?id=${PersonID}">
                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none"
                            stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                            <path d="M3 3h6l3 9 3-6h6"/>
                            <path d="M3 21v-4a4 4 0 0 1 4-4h14"/>
                        </svg>
                        Show Person License History
                    </a>
                    <a href="/Sections/DetainRelseLicense/Releasedetainlicense.html?licenseId=${r.licenseID}"
                        class="release-link ${r.isReleased ? 'link-disabled' : ''}">
                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none"
                            stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                            <rect x="3" y="11" width="18" height="11" rx="2" ry="2"/>
                            <path d="M7 11V7a5 5 0 0 1 9.9-1"/>
                        </svg>
                        Release Detained License
                    </a>
                </div>
            </div>
        </td>
    `;

    return tr;
}

/* ── Pagination ───────────────────────────────────────────────── */
function goToPage(page) {
    if (page < 1 || page > totalPages) return;
    currentPage = page;
    renderTable();
}

function updatePagination() {
    const start = (currentPage - 1) * PAGE_SIZE + 1;
    const end   = Math.min(currentPage * PAGE_SIZE, filtered.length);

    document.getElementById('pageInfo').textContent =
        `Page ${currentPage} of ${totalPages}`;
    document.getElementById('totalCount').textContent =
        filtered.length > 0 ? `${start}–${end} of ${filtered.length} records` : '0 records';

    document.getElementById('btnFirst').disabled = currentPage === 1;
    document.getElementById('btnPrev').disabled  = currentPage === 1;
    document.getElementById('btnNext').disabled  = currentPage === totalPages;
    document.getElementById('btnLast').disabled  = currentPage === totalPages;

    buildPageNumbers();
}

function buildPageNumbers() {
    const container = document.getElementById('pgNumbers');
    container.innerHTML = '';

    const delta = 2;
    const pages = new Set();

    pages.add(1);
    pages.add(totalPages);
    for (let i = currentPage - delta; i <= currentPage + delta; i++) {
        if (i >= 1 && i <= totalPages) pages.add(i);
    }

    const sorted = [...pages].sort((a, b) => a - b);
    let prev = null;

    sorted.forEach(p => {
        if (prev !== null && p - prev > 1) {
            const ellipsis = document.createElement('span');
            ellipsis.className = 'pg-ellipsis';
            ellipsis.textContent = '…';
            container.appendChild(ellipsis);
        }

        const btn = document.createElement('button');
        btn.className = 'pg-btn' + (p === currentPage ? ' active' : '');
        btn.textContent = p;
        btn.onclick = () => goToPage(p);
        container.appendChild(btn);

        prev = p;
    });
}

/* ── Helpers ──────────────────────────────────────────────────── */
function formatDate(iso) {
    if (!iso) return '—';
    const d = new Date(iso);
    return d.toLocaleDateString('en-GB', {
        day:   '2-digit',
        month: 'short',
        year:  'numeric'
    }) + ' ' + d.toLocaleTimeString('en-GB', {
        hour:   '2-digit',
        minute: '2-digit'
    });
}

function showError(msg) {
    const el = document.getElementById('errorMsg');
    el.textContent = msg;
    el.style.display = 'inline';
}

/* ── Expose globals needed by inline HTML handlers ───────────── */
window.goToPage      = goToPage;
window.onFilterByChange = function () {
    const val            = document.getElementById('filterBy').value;
    const textInput      = document.getElementById('filterInput');
    const releasedSelect = document.getElementById('isReleasedSelect');

    textInput.style.display      = 'none';
    releasedSelect.style.display = 'none';

    if (val === 'IsReleased') {
        releasedSelect.style.display = 'inline-block';
    } else if (val !== '') {
        textInput.style.display = 'inline-block';
    }

    // reset filter input values
    textInput.value      = '';
    releasedSelect.value = '';
    applyFilter();
};