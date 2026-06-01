import {showToast} from "./Global.js";


const ROWS_PER_PAGE = 6;

let currentPage = 1;
let totalPages  = 1;
let allPeople   = [];  

const filterBy     = document.getElementById("filterBy");
const filterInput  = document.getElementById("filterInput");
const genderSelect = document.getElementById("genderSelect");
const errorMsg     = document.getElementById("errorMsg");
const tableBody    = document.getElementById("peopleTableBody");
const pageInfo     = document.getElementById("pageInfo");
const totalCount   = document.getElementById("totalCount");
const pgNumbers    = document.getElementById("pgNumbers");

document.getElementById("btnFirst").addEventListener("click", () => goToPage(1));
document.getElementById("btnPrev").addEventListener("click",  () => goToPage(currentPage - 1));
document.getElementById("btnNext").addEventListener("click",  () => goToPage(currentPage + 1));
document.getElementById("btnLast").addEventListener("click",  () => goToPage(totalPages));

let isActive ;
window.addEventListener("load", () =>
{
    const userNameElement = document.getElementById("username");
    const storedName = localStorage.getItem("UserName");
    isActive = localStorage.getItem("isActive");

    if(userNameElement)
    {
        if(storedName)
        {
            userNameElement.textContent = storedName;
        }
        else 
        {
            userNameElement.textContent = "Guest"; // This sholud return null;
        }
    }
    loadFilteredPeople();
});

filterBy.addEventListener("change", function () {
    filterInput.value = "";
    filterInput.style.display  = "none";
    filterInput.style.border   = "1.5px solid var(--line)";
    genderSelect.style.display = "none";
    errorMsg.style.display     = "none";

    if (this.value === "Gender") {
        genderSelect.style.display = "block";
        genderSelect.value = "";
    } else if (this.value !== "") {
        filterInput.style.display = "block";
        filterInput.focus();
    }

    currentPage = 1;
    loadFilteredPeople();
});

filterInput.addEventListener("input", function () {
    const value = this.value;
    const type  = filterBy.value;
    let isValid = true;
    let message = "";

    if (type === "PersonID") {
        isValid = /^[0-9]*$/.test(value);
        message = "Person ID must contain numbers only";
    } else if (type === "Name") {
        isValid = /^[a-zA-Z\s]*$/.test(value);
        message = "Name must contain letters only";
    } else if (type === "Email") {
        isValid = value === "" || /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value);
        message = "Invalid email format";
    }

    if (!isValid && value !== "") {
        errorMsg.textContent   = message;
        errorMsg.style.display = "block";
        this.style.border      = "1.5px solid red";
    } else {
        errorMsg.style.display = "none";
        this.style.border      = "1.5px solid var(--line)";
    }

    clearTimeout(filterInput._timer);
    filterInput._timer = setTimeout(() => {
        currentPage = 1;
        loadFilteredPeople();
    }, 400);
});

genderSelect.addEventListener("change", () => {
    currentPage = 1;
    loadFilteredPeople();
});

async function loadFilteredPeople() {
    try {

        const filterType = filterBy.value;
        let value = filterType === "Gender"
            ? genderSelect.value
            : filterInput.value.trim();

        let url = "https://localhost:7223/api/People/All";
        if (filterType && value) {
            url = `https://localhost:7223/api/People/Filter?filterType=${encodeURIComponent(filterType)}&value=${encodeURIComponent(value)}`;
        }

        const response = await fetch(url);
        if (!response.ok) throw new Error("Failed to fetch");

        allPeople = await response.json();
        totalPages = Math.max(1, Math.ceil(allPeople.length / ROWS_PER_PAGE));

        renderPage(currentPage);
        renderPagination();

    } catch (err) {
        console.error(err);
        tableBody.innerHTML = `
            <tr><td colspan="7" style="text-align:center;color:#a32d2d;padding:24px;">
                Failed to load data.
            </td></tr>`;
    }
}

function renderPage(page) {
    currentPage = Math.min(Math.max(1, page), totalPages);

    const start = (currentPage - 1) * ROWS_PER_PAGE;
    const slice = allPeople.slice(start, start + ROWS_PER_PAGE);

    tableBody.innerHTML = "";

    if (slice.length === 0) {
        tableBody.innerHTML = `
            <tr><td colspan="7" style="text-align:center;color:var(--muted);padding:32px;">
                No records found.
            </td></tr>`;
        return;
    }

    slice.forEach(person => {
        const fullName    = [person.firstName, person.secondName,person.thirdName, person.lastName]
                            .filter(Boolean).join(" ");
        const genderClass = person.gender === "Male" ? "badge-male" : "badge-female";

        tableBody.innerHTML += `
            <tr>
                <td>${person.personID}</td>
                <td>${person.nationalNo}</td>
                <td>${fullName}</td>
                <td><span class="badge ${genderClass}">${person.gender}</span></td>
                <td>${person.nationality || "—"}</td>
                <td class="email-cell">${person.email || "—"}</td>
                <td>${makeOptionsBtns()}</td>
            </tr>`;
    });

    // تحديث info
    const from = start + 1;
    const to   = Math.min(start + ROWS_PER_PAGE, allPeople.length);
    pageInfo.textContent  = `Page ${currentPage} of ${totalPages}`;
    totalCount.textContent = `Showing ${from}–${to} of ${allPeople.length} people`;
}

function goToPage(page) {
    if (page < 1 || page > totalPages) return;
    renderPage(page);
    renderPagination();
}

function renderPagination() {
    pgNumbers.innerHTML = "";

    document.getElementById("btnFirst").disabled = currentPage === 1;
    document.getElementById("btnPrev").disabled  = currentPage === 1;
    document.getElementById("btnNext").disabled  = currentPage === totalPages;
    document.getElementById("btnLast").disabled  = currentPage === totalPages;

    // نافذة الأرقام: تظهر 5 أرقام max حول الصفحة الحالية
    const range  = 2;
    const start  = Math.max(1, currentPage - range);
    const end    = Math.min(totalPages, currentPage + range);

    if (start > 1) {
        pgNumbers.appendChild(makePageBtn(1));
        if (start > 2) pgNumbers.appendChild(makeEllipsis());
    }

    for (let i = start; i <= end; i++) {
        pgNumbers.appendChild(makePageBtn(i));
    }

    if (end < totalPages) {
        if (end < totalPages - 1) pgNumbers.appendChild(makeEllipsis());
        pgNumbers.appendChild(makePageBtn(totalPages));
    }
}

function makePageBtn(n) {
    const btn = document.createElement("button");
    btn.className = "pg-btn pg-num" + (n === currentPage ? " active" : "");
    btn.textContent = n;
    btn.onclick = () => goToPage(n);
    return btn;
}

function makeEllipsis() {
    const span = document.createElement("span");
    span.className = "pg-ellipsis";
    span.textContent = "…";
    return span;
}

function makeOptionsBtns() {
    return `
        <div class="options">
            <button class="btn-info">
                <svg viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.7">
                    <circle cx="8" cy="8" r="6"/>
                    <line x1="8" y1="7" x2="8" y2="11"/>
                    <circle cx="8" cy="5" r=".6" fill="currentColor" stroke="none"/>
                </svg>Info
            </button>
            <button class="btn-edit">
                <svg viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.7">
                    <path d="M11 2.5l2.5 2.5-7 7H4v-2.5l7-7z"/>
                </svg>Edit
            </button>
            <button class="btn-delete">
                <svg viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.7">
                    <polyline points="3,5 13,5"/>
                    <path d="M6 5V3h4v2"/>
                    <path d="M5 5l.7 8h4.6l.7-8"/>
                </svg>Delete
            </button>
        </div>`;
}

tableBody.addEventListener("click", function (e) {

    // Info
    const infoBtn = e.target.closest(".btn-info");
    if (infoBtn) {
        const personID = infoBtn.closest("tr").cells[0].textContent.trim();
        window.location.href = `/Sections/People/personInfo.html?id=${personID}`;
        return;
    }

    // Edit
    const editBtn = e.target.closest(".btn-edit");
    if (editBtn) {
        const personID = editBtn.closest("tr").cells[0].textContent.trim();
        console.log("Editing person:", personID);
        window.location.href = `/Sections/People/editPerson.html?id=${personID}`; // 
        return;
    }

    // Delete
    const deleteBtn = e.target.closest(".btn-delete");
    if (deleteBtn) {
        const row      = deleteBtn.closest("tr");
        const personID = row.cells[0].textContent.trim();
        const fullName = row.cells[2].textContent.trim();

        if(isActive === "false")
        {
            showToast("Your not active user in system, you can't delete any person, please contact with admin to active your account", "error");
            return;
        }
        if (confirm(`Are you sure you want to delete "${fullName}" (ID: ${personID})?`)) {
            deletePerson(personID, row);
        }
    }
});

async function deletePerson(id, rowElement) {
    try {
        const res = await fetch(`https://localhost:7223/api/People/${id}`, { method: "DELETE" });
        if (res.ok) {
            // أزل من المصفوفة وأعد الرسم
            allPeople = allPeople.filter(p => String(p.personID) !== String(id));
            totalPages = Math.max(1, Math.ceil(allPeople.length / ROWS_PER_PAGE));
            if (currentPage > totalPages) currentPage = totalPages;
            renderPage(currentPage);
            renderPagination();
        } else {
            showToast("Failed to delete the person.", "error");
        }
    } catch (err) {
        console.error(err);
        showToast("An error occurred while trying to delete.", "error");
    }
}