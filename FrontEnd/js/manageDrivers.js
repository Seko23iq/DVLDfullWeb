/* ===================================================================
    manageDrivers.js
    Fetches all drivers and renders them in the table
    with client-side filtering and pagination.
=================================================================== */

const API_URL = 'https://localhost:7223/api/Driver/GetAllDrivers';

const PAGE_SIZE = 10;

let allRecords  = [];
let filtered    = [];
let currentPage = 1;
let totalPages  = 1;

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
}

function applyFilter() {
    const filterBy = document.getElementById('filterBy').value;
    const textVal  = document.getElementById('filterInput').value.trim().toLowerCase();

    filtered = allRecords.filter(r => {
        if (!filterBy || !textVal) return true;

        switch (filterBy) {
            case 'DriverID':  return String(r.driverID).includes(textVal);
            case 'PersonID':  return String(r.personID).includes(textVal);
            case 'NationalNo': return r.nationalNo.toLowerCase().includes(textVal);
            case 'FullName':  return r.fullName.toLowerCase().includes(textVal);
            default:          return true;
        }
    });

    currentPage = 1;
    renderTable();
}

/* ── Render ───────────────────────────────────────────────────── */
function renderTable() {
    totalPages  = Math.max(1, Math.ceil(filtered.length / PAGE_SIZE));
    currentPage = Math.min(currentPage, totalPages);

    const start = (currentPage - 1) * PAGE_SIZE;
    const page  = filtered.slice(start, start + PAGE_SIZE);

    const tbody = document.getElementById('driversTableBody');
    tbody.innerHTML = '';

    if (page.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="6" style="text-align:center; padding:32px; color:var(--muted); font-size:14px;">
                    No records found.
                </td>
            </tr>`;
    } else {
        page.forEach(r => tbody.appendChild(buildRow(r)));
    }

    updatePagination();
}

function buildRow(r) {
    const tr = document.createElement('tr');

    const licensesBadge = `
        <span class="badge-licenses ${r.numberOfActiveLicenses === 0 ? 'zero' : ''}">
            ${r.numberOfActiveLicenses}
        </span>`;

    tr.innerHTML = `
        <td>${r.driverID}</td>
        <td>${r.personID}</td>
        <td>${r.nationalNo}</td>
        <td>${r.fullName}</td>
        <td class="date-cell">${formatDate(r.createdDate)}</td>
        <td>${licensesBadge}</td>
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

    const delta  = 2;
    const pages  = new Set();

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
            ellipsis.className   = 'pg-ellipsis';
            ellipsis.textContent = '…';
            container.appendChild(ellipsis);
        }

        const btn = document.createElement('button');
        btn.className   = 'pg-btn' + (p === currentPage ? ' active' : '');
        btn.textContent = p;
        btn.onclick     = () => goToPage(p);
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
    });
}

function showError(msg) {
    const el = document.getElementById('errorMsg');
    if (!el) return;
    el.textContent    = msg;
    el.style.display  = 'inline';
}

/* ── Expose globals needed by inline HTML handlers ───────────── */
window.goToPage   = goToPage;
window.applyFilter = applyFilter;