document.addEventListener("DOMContentLoaded", function () {

    // 1. Show / hide password (any button with class "toggle-password")
    document.querySelectorAll(".toggle-password").forEach(function (btn) {
        btn.addEventListener("click", function () {
            var input = btn.closest(".input-group").querySelector("input");
            var icon = btn.querySelector("i");
            var isHidden = input.type === "password";

            input.type = isHidden ? "text" : "password";
            icon.classList.toggle("bi-eye", !isHidden);
            icon.classList.toggle("bi-eye-slash", isHidden);
        });
    });

    // 2. Auto-dismiss success alerts after 5 seconds
    document.querySelectorAll(".alert.auto-dismiss").forEach(function (alertEl) {
        setTimeout(function () {
            var instance = bootstrap.Alert.getOrCreateInstance(alertEl);
            instance.close();
        }, 5000);
    });

    // 3. Confirmation dialog for any form with data-confirm="message"
    document.querySelectorAll("form[data-confirm]").forEach(function (form) {
        form.addEventListener("submit", function (e) {
            if (!window.confirm(form.getAttribute("data-confirm"))) {
                e.preventDefault();
            }
        });
    });

    // 4. Delete confirmation modal (Admin): fills in the complaint id and tracking number
    //    from the button that opened it
    var deleteModal = document.getElementById("deleteModal");
    if (deleteModal) {
        deleteModal.addEventListener("show.bs.modal", function (event) {
            var button = event.relatedTarget;
            if (!button) return;

            document.getElementById("deleteComplaintId").value = button.getAttribute("data-id");
            document.getElementById("deleteTrackingNo").textContent = button.getAttribute("data-tracking");
        });
    }
});
