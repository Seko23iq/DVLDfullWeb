// ═══════════════════════════════════════════════
//  addPerson.js  —  Add Person Page
// ═══════════════════════════════════════════════

const API_URL = "https://localhost:7223/api/People/AddPerson";

// ── DOM refs ──────────────────────────────────
const btnSave      = document.getElementById("btnSave");
const imageInput   = document.getElementById("imageInput");
const imagePreview = document.getElementById("imagePreview");
const toast        = document.getElementById("toast");


window.addEventListener("load", () => {
    const userNameElement = document.getElementById("username");
    const storedName = localStorage.getItem("UserName");

    if(userNameElement)
    {
        if(storedName)
        {
            userNameElement.textContent = storedName;
        }
        else 
        {
            userNameElement.textContent = "Guest";
        }
    }
});

// ── Image preview ─────────────────────────────
imageInput.addEventListener("change", function () {
    const file = this.files[0];
    if (!file) return;

    if (file.size > 5 * 1024 * 1024) {
        showToast("Image must be smaller than 5 MB.", "error");
        this.value = "";
        return;
    }

    const reader = new FileReader();
    reader.onload = e => imagePreview.src = e.target.result;
    reader.readAsDataURL(file);
});

// ── Validation rules ──────────────────────────
const rules = {
    nationalNo:  { required: true,  pattern: /^[a-zA-Z0-9]+$/,                       message: "Letters and numbers only." },
    gender:      { required: true,                                                     message: "Please select a gender." },
    firstName:   { required: true,  pattern: /^[a-zA-Z\u0600-\u06FF\s]+$/,            message: "Letters only." },
    secondName:  { required: true,  pattern: /^[a-zA-Z\u0600-\u06FF\s]+$/,            message: "Letters only." },
    thirdName:   { required: false, pattern: /^[a-zA-Z\u0600-\u06FF\s]*$/,            message: "Letters only." },
    lastName:    { required: true,  pattern: /^[a-zA-Z\u0600-\u06FF\s]+$/,            message: "Letters only." },
    dateOfBirth: { required: true,                                                     message: "Date of birth is required." },
    address:     { required: false },
    phone:       { required: false, pattern: /^\+?[0-9\s\-().]{7,20}$/,               message: "Invalid phone number." },
    email:       { required: false, pattern: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,            message: "Invalid email format." },
};

// ── Validate single field ─────────────────────
function validateField(id) {
    const rule  = rules[id];
    if (!rule) return true;

    const el    = document.getElementById(id);
    if (!el) return true;

    const errEl = document.getElementById(`err-${id}`);
    const val   = el.value.trim();

    let error = "";

    if (rule.required && val === "") {
        error = "This field is required.";
    } else if (rule.pattern && val !== "" && !rule.pattern.test(val)) {
        error = rule.message;
    }

    if (error) {
        el.classList.add("invalid");
        el.classList.remove("valid");
        if (errEl) errEl.textContent = error;
        return false;
    } else {
        el.classList.remove("invalid");
        if (val !== "") el.classList.add("valid");
        if (errEl) errEl.textContent = "";
        return true;
    }
}

// ── Live validation ───────────────────────────
Object.keys(rules).forEach(id => {
    const el = document.getElementById(id);
    if (!el) return;
    el.addEventListener("blur",  () => validateField(id));
    el.addEventListener("input", () => {
        if (el.classList.contains("invalid")) validateField(id);
    });
});

// ── Validate all ─────────────────────────────
function validateAll() {
    return Object.keys(rules)
        .map(id => validateField(id))
        .every(Boolean);
}

// ── Build payload — يطابق JSON بالضبط ────────
function buildPayload() {
    return {
        nationalNo:  document.getElementById("nationalNo").value.trim(),
        firstName:   document.getElementById("firstName").value.trim(),
        secondName:  document.getElementById("secondName").value.trim(),
        thirdName:   document.getElementById("thirdName").value.trim()  || null,
        lastName:    document.getElementById("lastName").value.trim(),
        dateOfBirth: document.getElementById("dateOfBirth").value,
        gender:      document.getElementById("gender").value,
        address:     document.getElementById("address").value.trim()    || null,
        phone:       document.getElementById("phone").value.trim()      || null,
        email:       document.getElementById("email").value.trim()      || null,
        nationality: document.getElementById("nationality").value,
        imagePath:  document.getElementById("imageInput").value.trim() || null,
    };
}

// ── Reset button to original state ───────────
function resetBtn() {
    btnSave.disabled = false;
    btnSave.innerHTML = `
        <svg viewBox="0 0 20 20" fill="none" stroke="currentColor" stroke-width="1.8">
            <path d="M4 10.5l4.5 4.5 7.5-8"/>
        </svg>
        Save Person`;
}

btnSave.addEventListener("click", async () => {
    if (!validateAll()) {
        showToast("Please fix the errors before saving.", "error");
        return;
    }

    btnSave.disabled = true;
    btnSave.innerHTML = `
        <svg viewBox="0 0 20 20" fill="none" stroke="currentColor"
                stroke-width="1.8" style="animation:spin .7s linear infinite;width:15px;height:15px">
            <circle cx="10" cy="10" r="7" stroke-dasharray="30" stroke-dashoffset="10"/>
        </svg>
        Saving…`;

    try {
            
        const payload = buildPayload();
        const file = imageInput.files[0];
        
        let response;

        if (file) {
            // ── إرسال مع صورة ──
            const formData = new FormData();

            formData.append("image", file);
            Object.entries(payload).forEach(([k, v]) => {
                formData.append(k, v ?? "");
            });

            response = await fetch(API_URL, {
                method: "POST",
                body: formData
            });

        } else {
            // ── إرسال بدون صورة ──
            response = await fetch(API_URL, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(payload),
            });
        }

        // 🔥 حماية إضافية
        if (!response) {
            throw new Error("No response from server");
        }

        // ── معالجة الاستجابة ──
        if (!response.ok) {
            let errorMessage = `Server error (${response.status})`;

            try {
                const errBody = await response.json();
                errorMessage = errBody.message
                    || errBody.title
                    || JSON.stringify(errBody.errors ?? errBody)
                    || errorMessage;
            } catch {
                errorMessage = response.statusText || errorMessage;
            }

            throw new Error(errorMessage);
        }

        showToast("Person added successfully!", "success");

        setTimeout(() => {
            window.location.href = "/Sections/People/people.html";
        }, 1400);

    } catch (error) {
        console.error("Add Person Error:", error);
        showToast(error.message || "Failed to add person.", "error");
        resetBtn();
    }
}
);

// ── Toast helper ──────────────────────────────
let toastTimer;
function showToast(message, type = "success") {
    clearTimeout(toastTimer);
    toast.textContent = message;
    toast.className   = `toast ${type} show`;
    toastTimer = setTimeout(() => {
        toast.classList.remove("show");
    }, 3000);
}

// ── Spinner keyframe (inject once) ───────────
const style = document.createElement("style");
style.textContent = `@keyframes spin { to { transform: rotate(360deg); } }`;
document.head.appendChild(style);


async function loadCountries() {
    try {
        const response = await fetch("https://localhost:7223/api/Country/AllCountries");
        const countries = await response.json();

        const select = document.getElementById("nationality");
        countries.forEach((country, index) => {
            const option = document.createElement("option");
            option.value = index + 1; // ← index يبدأ من 1
            option.textContent = country;
            select.appendChild(option);
        });

    } catch (error) {
        console.error("Failed to load countries:", error);
    }
}
// استدعها عند تحميل الصفحة
document.addEventListener("DOMContentLoaded", () => {
    loadCountries();
});