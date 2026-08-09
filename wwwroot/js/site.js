// WebTerriane site.js — small, targeted behaviors only.
// Bootstrap bundle (collapse, etc.) is loaded separately in _Layout.cshtml.

document.addEventListener('DOMContentLoaded', function () {

    // Navbar gains a shadow once the page scrolls past the hero's top edge.
    var navbar = document.getElementById('wtNavbar');
    if (navbar) {
        window.addEventListener('scroll', function () {
            navbar.style.boxShadow = window.scrollY > 12
                ? '0 4px 20px rgba(10,27,51,0.08)'
                : 'none';
        }, { passive: true });
    }

    // Stat counters animate up once scrolled into view (runs once per element).
    var statEls = document.querySelectorAll('.wt-stat-num');
    if (statEls.length && 'IntersectionObserver' in window) {
        var reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

        var observer = new IntersectionObserver(function (entries, obs) {
            entries.forEach(function (entry) {
                if (!entry.isIntersecting) return;
                var el = entry.target;
                var target = parseInt(el.getAttribute('data-count'), 10) || 0;

                if (reduceMotion) {
                    el.textContent = target;
                    obs.unobserve(el);
                    return;
                }

                var current = 0;
                var duration = 1200;
                var stepTime = 16;
                var steps = duration / stepTime;
                var increment = target / steps;

                var timer = setInterval(function () {
                    current += increment;
                    if (current >= target) {
                        el.textContent = target;
                        clearInterval(timer);
                    } else {
                        el.textContent = Math.floor(current);
                    }
                }, stepTime);

                obs.unobserve(el);
            });
        }, { threshold: 0.4 });

        statEls.forEach(function (el) { observer.observe(el); });
    }
});
