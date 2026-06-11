function updateClock() {
    const now = new Date();
    const el = document.getElementById('clock-display');
    if (el) el.textContent = now.toLocaleDateString('vi-VN', {
        weekday: 'short', day: '2-digit', month: '2-digit'
    }) + ' · ' + now.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
}
updateClock(); setInterval(updateClock, 1000);

/* ── Sidebar mobile ── */
function openSidebar() {
    document.getElementById('sidebar').classList.add('show');
    document.getElementById('sidebarOverlay').classList.add('show');
}
function closeSidebar() {
    document.getElementById('sidebar').classList.remove('show');
    document.getElementById('sidebarOverlay').classList.remove('show');
}

/* ── Logout dropdown ── */
function toggleLogout() {
    const el = document.getElementById('DangXuat');
    el.style.display = el.style.display === 'block' ? 'none' : 'block';
}
document.addEventListener('click', function (e) {
    if (!e.target.closest('.sidebar-footer'))
        document.getElementById('DangXuat').style.display = 'none';
});

/* ── Bootstrap tooltips ── */
const tooltipEls = document.querySelectorAll('[data-bs-toggle="tooltip"]');
tooltipEls.forEach(el => new bootstrap.Tooltip(el));
function toggleSidebar() {
    const sidebar = document.getElementById('sidebar');
    const mainContent = document.querySelector('.main-content');
    const topbar = document.querySelector('.topbar');
    const isMobile = window.innerWidth < 992; // ← sửa từ 800 → 992

    if (isMobile) {
        sidebar.classList.contains('show') ? closeSidebar() : openSidebar();
    } else {
        sidebar.classList.toggle('collapsed');
        mainContent.classList.toggle('collapsed');
        topbar.classList.toggle('collapsed');
    }
}
/* ══════════════════════════════
TAB SYSTEM (WinForm style)
══════════════════════════════ */
const tabs = []; // { id, name, icon, url }
let activeTabId = null;

function openTab(url, name, icon) {
    const existing = tabs.find(t => t.url === url);
    if (existing) { switchTab(existing.id); return; }

    const id = 'tab_' + Date.now();
    tabs.push({ id, name, icon, url });

    const $tab = $(`
        <div class="tab-item" id="ti_${id}" data-id="${id}">
            <i class="tab-icon ${icon}"></i>
            <span class="tab-name">${name}</span>
            <span class="tab-close" data-id="${id}"><i class="fas fa-times"></i></span>
        </div>
    `);
    $('#tabBar').append($tab);

    // Tạo iframe và gán onload trực tiếp
    const $panel = $(`<div class="tab-panel" id="tp_${id}"></div>`);
    const iframe = document.createElement('iframe');
    iframe.src = url;
    iframe.loading = 'lazy';

    // ← Gán onload trực tiếp vào element
    iframe.onload = function () {
        try {
            const iframeDoc = this.contentDocument || this.contentWindow.document;

            // Inject CSS files
            const cssFiles = [
                '/css/TheoDoiHangXuat/fontawesome.pro.6.0.0.css',
                '/css/DashBoard/shared.css',
            ];
            cssFiles.forEach(href => {
                // Tránh inject trùng
                const already = iframeDoc.querySelector(`link[href="${href}"]`);
                if (already) return;
                const link = iframeDoc.createElement('link');
                link.rel = 'stylesheet';
                link.href = href;
                iframeDoc.head.appendChild(link);
            });
        } catch (e) {
            // Cross-origin sẽ bị block ở đây
            console.warn('Không thể inject CSS vào iframe:', e);
        }
    };

    $panel.append(iframe);
    $('#tabPanels').append($panel);
    $('.main-content').addClass('has-tabs');
    switchTab(id);
}

function switchTab(id) {
    activeTabId = id;

    // Tab buttons
    $('.tab-item').removeClass('active');
    $(`#ti_${id}`).addClass('active');

    // Panels
    $('.tab-panel').removeClass('active');
    $(`#tp_${id}`).addClass('active');

    // Scroll tab vào view
    const tabEl = document.getElementById('ti_' + id);
    if (tabEl) tabEl.scrollIntoView({ behavior: 'smooth', inline: 'nearest', block: 'nearest' });
}

function closeTab(id) {
    const idx = tabs.findIndex(t => t.id === id);
    if (idx === -1) return;

    tabs.splice(idx, 1);
    $(`#ti_${id}`).remove();
    $(`#tp_${id}`).remove();

    if (tabs.length === 0) {
        // Hết tab → trả về dashboard
        $('.main-content').removeClass('has-tabs');
        activeTabId = null;

        // Reset active sidebar về mục đầu tiên
        $('.sidebar-link').removeClass('active');
        $('.sidebar-link').first().addClass('active');
    } else if (activeTabId === id) {
        const nextIdx = Math.min(idx, tabs.length - 1);
        switchTab(tabs[nextIdx].id);
    }
}

/* Click tab button → switch */
$(document).on('click', '.tab-item', function (e) {
    if (!$(e.target).closest('.tab-close').length) {
        switchTab($(this).data('id'));
    }
});

/* Click nút close */
$(document).on('click', '.tab-close', function (e) {
    e.stopPropagation();
    closeTab($(this).data('id'));
});
function toggleGroup(header) {
    // Nếu sidebar đang collapsed thì không làm gì
    if (document.getElementById('sidebar').classList.contains('collapsed')) return;

    const group = header.closest('.nav-group');
    group.classList.toggle('closed');
}

// Mặc định đóng tất cả trừ group có link active
$(document).ready(function () {
    // Đóng tất cả trước
    $('.nav-group').addClass('closed');

    // Mở group chứa link active
    const $activeLink = $('.sidebar-link.active').first();
    if ($activeLink.length) {
        $activeLink.closest('.nav-group').removeClass('closed');
    } else {
        // Nếu không có active thì mở group đầu tiên
        $('.nav-group').first().removeClass('closed');
    }
});

// Khi click link → tự mở group cha và đóng group khác (tuỳ chọn)
$(document).on('click', '.sidebar-link', function () {
    const $group = $(this).closest('.nav-group');
    // Mở group chứa link vừa click (phòng trường hợp group đang đóng)
    $group.removeClass('closed');
});

/* ── Auth & Permission ── */
$(document).ready(function () {
    let user = localStorage.getItem('username1');
    $('.user').text(user || 'Người dùng');
    if (user) {
        $('#UserAvatarSidebar').text(user.charAt(0).toUpperCase());
    }
    localStorage.setItem('isCheckDN', 'true');

    $('#logout-link').on('click', function () {
        localStorage.removeItem('username1');
        localStorage.removeItem('isCheckDN');
        localStorage.removeItem('password1');
    });


});

const StatCards = [
    { Icon: 'fas fa-boxes-stacking', Bg: '#d1fae5', Color: '#0ea472', Label: 'Đơn hàng hôm nay' },
    { Icon: 'fas fa-box-check', Bg: '#dbeafe', Color: '#1a56db', Label: 'Thùng đã đóng' },
    { Icon: 'fas fa-warehouse', Bg: '#fef3c7', Color: '#d97706', Label: 'Tồn kho NPL' },
    { Icon: 'fas fa-shield-check', Bg: '#ffe4e6', Color: '#e11d48', Label: 'Chờ phê duyệt' },
];
var ModuleData = [];

function renderAll() {

    // ── 1. Stat Cards ──
    let statHtml = '<div class="row g-3 mb-2 fade-in d-none">';
    StatCards.forEach(s => {
        statHtml += `
        <div class="col-6 col-xl-3">
            <div class="stat-card d-flex align-items-center gap-3">
                <div class="stat-icon" style="background:${s.Bg};color:${s.Color}">
                    <i class="${s.Icon}"></i>
                </div>
                <div>
                    <div class="stat-value">—</div>
                    <div class="stat-label">${s.Label}</div>
                    <div class="stat-delta neutral">Đang cập nhật</div>
                </div>
            </div>
        </div>`;
    });
    statHtml += '</div>';

    // ── 2. Group ModuleData theo Section ──
    const sections = [];
    const sectionMap = {};
    const navGroups = [];
    const navGroupMap = {};

    ModuleData.forEach(m => {
        if (!sectionMap[m.Section]) {
            const s = { Id: m.Section, Title: m.SectionTitle, Icon: m.SectionIcon, Bg: m.SectionBg, Color: m.SectionColor, Items: [] };
            sectionMap[m.Section] = s;
            sections.push(s);
        }
        sectionMap[m.Section].Items.push(m);

        if (!navGroupMap[m.Section]) {
            const g = { Section: m.Section, Title: m.SectionTitle, Icon: m.SectionIcon, Items: [] };
            navGroupMap[m.Section] = g;
            navGroups.push(g);
        }
        navGroupMap[m.Section].Items.push(m);
    });

    // ── 3. Render dashboard sections ──
    let sectionsHtml = '';
    sections.forEach((s, si) => {
        let cardsHtml = '';
        s.Items.forEach(m => {
            const classLink = m.Section == "section-pheduyet" ? "mod-card-pd" : ""

            const hiddenClass = m.HiddenDefault ? 'd-none' : '';
            cardsHtml += `
            <div class="col-6 col-md-4 col-lg-3 col-xl-2 ${hiddenClass}">
                <a id="${m.IdCard}" href="${m.Href}" data-moduleid="${m.Module}" class="mod-card ${m.Color} ${classLink}">
                    <div class="mod-icon"><i class="${m.Icon}"></i></div>
                    <div class="mod-name">${m.Text}</div>
                    <i class="fas fa-arrow-up-right mod-arrow"></i>
                </a>
            </div>`;
        });
        sectionsHtml += `
        <div id="${s.Id}" class="fade-in delay-${si + 1}">
            <div class="section-heading">
                <div class="section-heading-icon" style="background:${s.Bg};color:${s.Color}">
                    <i class="${s.Icon}"></i>
                </div>
                <h2>${s.Title}</h2>
                <div class="section-heading-line"></div>
            </div>
            <div class="row g-3">${cardsHtml}</div>
        </div>`;
    });

    // ── 4. Đổ vào dashboardArea ──
    $('#dashboardArea').html(statHtml + '<div id="dashboard-sections">' + sectionsHtml + '</div>');

    // ── 5. Render sidebar nav ──
    let navHtml = '';
    navGroups.forEach(g => {
        let linksHtml = '';

        g.Items.forEach(m => {
            const classLink = m.Section == "section-pheduyet" ? "sidebar-link-pd" : ""
            const hiddenClass = m.HiddenDefault ? 'd-none' : '';
            linksHtml += `
            <a href="${m.Href}" id="${m.IdNav}" data-moduleid="${m.Module}" class="sidebar-link ${hiddenClass} ${classLink}">
                <span class="nav-icon"><i class="${m.Icon}"></i></span>
                <span class="titleBar">${m.Text}</span>
            </a>`;
        });
        navHtml += `
        <div class="nav-group closed">
            <div class="nav-group-header" onclick="toggleGroup(this)">
                <span class="nav-icon"><i class="${g.Icon}"></i></span>
                <span class="nav-group-name">${g.Title}</span>
                <i class="fas fa-chevron-down nav-chevron"></i>
            </div>
            <div class="nav-group-body">${linksHtml}</div>
        </div>`;
    });
    $('#sidebarNav').html(navHtml);
}


// ═══════════════════════════════════════════════════════
//  PERMISSION
// ═══════════════════════════════════════════════════════
function applyPermissions(data) {
    const aliasMap = {};
    ModuleData.forEach(m => {
        if (!aliasMap[m.Module]) aliasMap[m.Module] = [];
        aliasMap[m.Module].push(m);
    });

    Object.entries(aliasMap).forEach(([moduleCode, items]) => {
        const perm = data.find(x => x.Module === moduleCode);
        if (perm?.IsCheckXem === false) {
            items.forEach(m => {
                $('#' + m.IdCard).closest('.col-6, .col-md-4, .col-lg-3, .col-xl-2').hide();
                $('#' + m.IdNav).hide();
            });
        }
    });

    // Ẩn section nếu tất cả card bị ẩn
    ModuleData.reduce((acc, m) => { acc.add(m.Section); return acc; }, new Set()).forEach(sectionId => {
        const $section = $('#' + sectionId);
        const allHidden = $section.find('.col-6').toArray().every(el => $(el).is(':hidden'));
        if (allHidden) $section.hide();
    });

    // Ẩn nav group nếu tất cả link bị ẩn
    $('.nav-group').each(function () {
        const $links = $(this).find('.sidebar-link');
        const allHidden = $links.length > 0 && $links.toArray().every(el => $(el).is(':hidden'));
        if (allHidden) $(this).hide();
    });
}


// ═══════════════════════════════════════════════════════
//  CLICK: mở tab từ card hoặc nav link
// ═══════════════════════════════════════════════════════
$(document).on('click', '.sidebar-link[href], .mod-card[href]', function (e) {
    localStorage.setItem("moduleID", $(this).data("moduleid"))
    //if ($(this).hasClass('sidebar-link-pd') || $(this).hasClass('mod-card-pd')) {
    //    return;
    //}
    //let url = $(this).attr('href');
    //if (!url || url === '#' || url.startsWith('javascript')) return;
    //e.preventDefault();

    //url = url.replace(/\/$/, '');
    //const name = $(this).find('.titleBar').text().trim() || $(this).find('.mod-name').text().trim() || 'Trang';
    //const icon = $(this).find('.nav-icon i').attr('class') || $(this).find('.mod-icon i').attr('class') || 'fas fa-circle';

    //$('.sidebar-link').removeClass('active');
    //$('.sidebar-link[href]').filter(function () {
    //    return $(this).attr('href').replace(/\/$/, '') === url;
    //}).addClass('active');

    //openTab(url, name, icon);
    //if (window.innerWidth <= 800) closeSidebar();
});

$(document).ready(function () {

    // Accordion: đóng tất cả, mở group đầu tiên
    $('.nav-group').addClass('closed');
    $('.nav-group').first().removeClass('closed');

    $(document).on('click', '.sidebar-link', function () {
        $(this).closest('.nav-group').removeClass('closed');
    });

    // Auth
    const user = localStorage.getItem('username1');
    $('.user').text(user || 'Người dùng');
    if (user) $('#UserAvatarSidebar').text(user.charAt(0).toUpperCase());
    localStorage.setItem('isCheckDN', 'true');

    $('#logout-link').on('click', function () {
        localStorage.removeItem('username1');
        localStorage.removeItem('isCheckDN');
        localStorage.removeItem('password1');
    });
    LoadModule(user)

});

async function LoadModule(user) {
    const url = `/api/Login/GetUserDashBoard?&Para1=${user}`;

    try {
        const response = await fetch(url)
        const data = await response.json()
        if (data.length == 0) {
            ModuleData = []
            return
        }
        ModuleData = data
        renderAll();
    } catch (err) {
        console.error(err)
    }
}