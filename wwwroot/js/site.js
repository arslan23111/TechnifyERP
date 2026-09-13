// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.addEventListener("DOMContentLoaded", function () {
    var currentPath = window.location.pathname.replace(/\/$/, "").toLowerCase() || "/";
    var links = Array.from(document.querySelectorAll(".app-navbar .nav-link[href]"));

    links.forEach(function (link) {
        link.classList.remove("sidebar-active");
        link.classList.remove("text-primary");
    });

    var activeLink = links
        .map(function (link) {
            var linkPath = new URL(link.href, window.location.origin).pathname.replace(/\/$/, "").toLowerCase() || "/";
            var matches = linkPath === "/"
                ? currentPath === "/"
                : currentPath === linkPath || currentPath.indexOf(linkPath + "/") === 0;
            return { link: link, linkPath: linkPath, matches: matches };
        })
        .filter(function (item) { return item.matches; })
        .sort(function (a, b) { return b.linkPath.length - a.linkPath.length; })[0];

    if (activeLink) activeLink.link.classList.add("sidebar-active");

    var sidebarPanel = document.querySelector(".role-admin .navbar-collapse, .role-faculty .navbar-collapse, .role-student .navbar-collapse");
    var sidebarShell = document.querySelector(".role-admin .app-navbar, .role-faculty .app-navbar, .role-student .app-navbar");

    function getSidebarScroller() {
        if (sidebarPanel && sidebarPanel.scrollHeight > sidebarPanel.clientHeight) return sidebarPanel;
        if (sidebarShell && sidebarShell.scrollHeight > sidebarShell.clientHeight) return sidebarShell;
        return sidebarPanel || sidebarShell;
    }

    function saveSidebarScroll() {
        var scroller = getSidebarScroller();
        if (scroller) sessionStorage.setItem("technify-sidebar-scroll", String(scroller.scrollTop));
    }

    if (sidebarPanel || sidebarShell) {
        var savedScroll = sessionStorage.getItem("technify-sidebar-scroll");
        if (savedScroll === null) savedScroll = localStorage.getItem("technify-sidebar-scroll");
        if (savedScroll !== null) {
            var restoreSidebarScroll = function () {
                var scroller = getSidebarScroller();
                if (scroller) scroller.scrollTop = Number(savedScroll);
            };
            requestAnimationFrame(restoreSidebarScroll);
            window.setTimeout(restoreSidebarScroll, 120);
        }

        [sidebarPanel, sidebarShell].filter(Boolean).forEach(function (element) {
            element.addEventListener("scroll", saveSidebarScroll, { passive: true });
        });

        document.querySelectorAll(".app-navbar a.nav-link").forEach(function (link) {
            link.addEventListener("click", function () {
                saveSidebarScroll();
            });
        });

        window.addEventListener("pagehide", saveSidebarScroll);
    }

    var toggle = document.getElementById("sidebarToggle");
    if (!toggle) return;
    if (localStorage.getItem("technify-sidebar") === "collapsed") document.body.classList.add("sidebar-collapsed");
    toggle.addEventListener("click", function () {
        document.body.classList.toggle("sidebar-collapsed");
        localStorage.setItem("technify-sidebar", document.body.classList.contains("sidebar-collapsed") ? "collapsed" : "open");
    });
});
