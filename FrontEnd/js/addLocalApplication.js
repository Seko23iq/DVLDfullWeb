import {formatDate, showToast} from "./Global.js";

const BASE_URL = "https://localhost:7223/api";

let currentPersonId = null;

const btnSearch = document.getElementById("btnSearch");


window.addEventListener("DOMContentLoaded", () => {
    setCurrentDate();
    setApplicationFees();
    setCreatedBy();
    loadLicenseClasses();
});

function setCurrentDate() {
    const today = new Date();
    document.getElementById("formAppDate").textContent = formatDate(today.toISOString());
}

async function setApplicationFees()
{
    let PaidFees = await GetPaidFessLocalApplication();
    document.getElementById("formAppFees").textContent = PaidFees.toString();
}

function setCreatedBy() {
    // Adjust this key to wherever your app stores the logged-in username
    const username = localStorage.getItem("UserName") || sessionStorage.getItem("username") || "—";
    document.getElementById("formCreatedBy").textContent = username;
}

async function loadLicenseClasses() {
    const select = document.getElementById("formLicenseClass");
    try {
        const response = await fetch(`${BASE_URL}/LocalDrivingApplication/LicenseClasses`);
        if (!response.ok) throw new Error("Failed to load license classes");
        
        const classes = await response.json();
        
        select.innerHTML = `<option value="" disabled selected>Select a class…</option>`;

        classes.forEach((cls, index) => {
            const option = document.createElement("option");

            option.value = index + 1; // أو cls إذا تريد
            option.textContent = cls;

            select.appendChild(option);
        });

    } catch (error) {
        console.error("Error loading license classes:", error);
        select.innerHTML = `<option value="" disabled selected>Failed to load classes</option>`;
    }
}

// Its the same in all LicenseClass Came it later from database and put it here
// ── 5. Auto-fill fees when license class changes ─────────────────
// document.getElementById("formLicenseClass").addEventListener("change", function () {
//     // const selected = this.options[this.selectedIndex];
//     // const fees = selected.dataset.fees;
//     // document.getElementById("formAppFees").textContent = fees ? `${fees} IQD` : "—";

//     // // Clear validation error if user picks a class
//     // this.classList.remove("error");
//     // document.getElementById("licenseClassErr").classList.add("hidden");
// });

btnSearch.addEventListener("click", () => 
    {
        loadPersonInfo();
    } 
);

document.getElementById("personIdInput").addEventListener("keydown", e => {
    if (e.key === "Enter") loadPersonInfo();
});

async function loadPersonInfo() {
    const personID = document.getElementById("personIdInput").value.trim();

    if (!personID) return;

    try {
        const response = await fetch(`${BASE_URL}/People/PeopleInfo/${personID}`);

        if (!response.ok) throw new Error("Person not found");

        const person = await response.json();
        showPersonInfo(person);

    } catch (error) {
        console.error("Error:", error);
        showNotFound();
    }
}

function showPersonInfo(person) {
    const card     = document.getElementById("personInfoCard");
    const notFound = document.getElementById("notFoundMsg");
    const form     = document.getElementById("addAppForm");

    // Reset visibility
    card.classList.add("hidden");
    notFound.classList.add("hidden");
    form.classList.add("hidden");
    clearFormErrors();

    if (!person) {
        showNotFound();
        return;
    }

    // Populate info card
    const fullName = [person.firstName, person.secondName, person.thirdName, person.lastName]
        .filter(Boolean).join(" ");

    document.getElementById("infoId").textContent       = person.personID;
    document.getElementById("infoName").textContent     = fullName;
    document.getElementById("infoNational").textContent = person.nationalNo;
    document.getElementById("infoGender").textContent   = person.gender;
    document.getElementById("infoEmail").textContent    = person.email;
    document.getElementById("infoDOB").textContent      = formatDate(person.dateOfBirth);
    document.getElementById("infoCountry").textContent  = person.nationality || "—";
    document.getElementById("infoAddress").textContent  = person.address || "—";

    // الصورة
    const img = document.querySelector(".personImage input[type='image']");
    if (img && person.imagePath) {
        img.src = "https://localhost:7223" + person.imagePath;
        img.alt = fullName;
    }
    else 
    {
        img.src = "/Images/defaultImage.webp";
        img.alt = "There is no image";
    }

    card.classList.remove("hidden");

    // Show & prepare application form
    document.getElementById("formLicenseClass").value = "";
    // document.getElementById("formAppFees").textContent = "—";
    form.classList.remove("hidden");

    currentPersonId = person.personID;

    // Scroll to form smoothly
    setTimeout(() => form.scrollIntoView({ behavior: "smooth", block: "start" }), 100);
}

function showNotFound() {
    document.getElementById("notFoundMsg").classList.remove("hidden");
    currentPersonId = null;
}

async function GetPaidFessLocalApplication() {
    try {
        let url = "https://localhost:7223/api/ApplicationTypes/All";

        const response = await fetch(url);
        if (!response.ok) throw new Error("Failed to fetch");

        let allApplicationTypes   = await response.json();

        const find = allApplicationTypes.find(s => s.id === 1)?.fees;

        return find;
    } catch (err) {
        console.error(err);
    }
}

async function buildLocalApplicationPayload() {
    let valid = true;

    const applicantPersonID = currentPersonId;
    const paidFees = await GetPaidFessLocalApplication();
    const licenseClassID = document.getElementById("formLicenseClass").value;
    const createdByUserID = localStorage.getItem("UserID");

    const feesInputElement = document.getElementById("formAppFees");

    if (isNaN(paidFees) || paidFees < 0) {
        feesInputElement.classList.add("error");
        valid = false;
    } else {
        feesInputElement.classList.remove("error");
    }

    if (!licenseClassID || isNaN(licenseClassID)) {
        valid = false;
    }

    if (!valid) return null;

    // ── Payload ────────────────────────────────────────────────────
    return {
        ApplicantPersonID: applicantPersonID,
        PaidFees: paidFees,
        CreatedByUserID: Number(createdByUserID),
        LicenseClassID: Number(licenseClassID)
    };
}



async function HasLicenseWithThisClass(personID, licenseClassID) {
    try {
        const response = await fetch(`${BASE_URL}/LocalDrivingApplication/HasLicenseWithThisClass?PersonID=${personID}&LicenseClassID=${licenseClassID}`);
        if (!response.ok) throw new Error("Failed to check existing licenses");
        const result = await response.json();
        return result;
    } catch (error) {
        console.error("Error checking existing licenses:", error);
        return false; // Assume no license if error occurs
    }
}

const btnSubmit = document.getElementById("btnSubmit");

btnSubmit.addEventListener("click", async () => {
    
    const personID = document.getElementById("personIdInput").value.trim();
    const licenseClassID = document.getElementById("formLicenseClass").value;

    const hasLicense = await HasLicenseWithThisClass(Number(personID), Number(licenseClassID));

    if(hasLicense)
    {
        showToast("You already have an active license application of this class.", "error");
        btnSubmit.disabled = false;
        return;
    }   

    submitForm();
})
async function submitForm() {
    clearFormErrors();

    const payload = await buildLocalApplicationPayload();
    if (!payload) return;

    const btn = document.getElementById("btnSubmit");
    btn.disabled = true;

    try {
        // ✅ استخدم FormData بدل JSON
        const formData = new FormData();
        formData.append("ApplicantPersonID", payload.ApplicantPersonID);
        formData.append("PaidFees",          payload.PaidFees);
        formData.append("CreatedByUserID",   payload.CreatedByUserID);
        formData.append("LicenseClassID",    payload.LicenseClassID);

        const response = await fetch(`${BASE_URL}/LocalDrivingApplication/Add`, {
            method: "POST",
            body: formData,
        });

        if (response.ok) {
            const result = await response.json();
            if (result?.newID) {
                document.getElementById("formAppIdValue").textContent = `#${result.newID}`;
            }
            showToast("Application submitted successfully!", "success");
            setTimeout(() => {
                window.location.href = "/Sections/Applications/LocalApplication/mainLocalApplication.html";
            }, 1400);
        } else {
            let msg = "Failed to submit application.";
            try {
                const errBody = await response.json();
                msg = errBody.message ?? errBody.title ?? msg;
            } catch { }
            showToast(msg, "error");
        }

    } catch (error) {
        console.error("Submit error:", error);
        showToast(error.message ?? "Network error. Please try again.", "error");
    } finally {
        btn.disabled = false;
    }
}
function resetAll() {
    document.getElementById("personIdInput").value = "";
    document.getElementById("personInfoCard").classList.add("hidden");
    document.getElementById("notFoundMsg").classList.add("hidden");
    document.getElementById("addAppForm").classList.add("hidden");
    document.getElementById("formAppIdValue").textContent = "Auto-generated";
    document.getElementById("formAppFees").textContent = "—";
    clearFormErrors();
    currentPersonId = null;
    window.scrollTo({ top: 0, behavior: "smooth" });
}

function clearFormErrors() {
    document.getElementById("formLicenseClass")?.classList.remove("error");
    document.getElementById("licenseClassErr")?.classList.add("hidden");
}

const btnCancel = document.getElementById("btnCancel");
btnCancel.addEventListener("click", () => {
    window.location.href = "/Sections/Applications/LocalApplication/mainLocalApplication.html";
})