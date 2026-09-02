// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.addEventListener("DOMContentLoaded", function () {
    var toggle = document.getElementById("sidebarToggle");
    if (!toggle) return;
    if (localStorage.getItem("technify-sidebar") === "collapsed") document.body.classList.add("sidebar-collapsed");
    toggle.addEventListener("click", function () {
        document.body.classList.toggle("sidebar-collapsed");
        localStorage.setItem("technify-sidebar", document.body.classList.contains("sidebar-collapsed") ? "collapsed" : "open");
    });
});
