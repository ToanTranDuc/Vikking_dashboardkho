 //UserID = localStorage.getItem("username1");
var modulesData = [];
var renderedTabs = [];
var arrCountPhieuChuaDuyet = [];
var arrAction = [
    { ModuleID: "M.12.01.00", Action: "GetPhieuCanDoi", IsFrm: true, isLink: false, contentType: 'grid' },
    { ModuleID: "M.12.02.00", Action: "GetPhieuBaoGia", IsFrm: true, isLink: false, contentType: 'grid' },
    { ModuleID: "M.12.03.00", Action: "GetPhieuMuaHang", IsFrm: true, isLink: false, contentType: 'grid' },
    { ModuleID: "M.21.00.00", Action: "GetPhieuDanhGia", IsFrm: false, isLink: false, contentType: 'grid' },
    { ModuleID: "M.04.01.00", Action: "GetDetailDH", IsFrm: false, isLink: false, contentType: 'grid' },
    { ModuleID: "M.24.00.00", Action: "/NguyenPhuLieu/GhiNhanKiemKeNL", IsFrm: false, isLink: true, contentType: 'external' },
    { ModuleID: "M.25.00.00", Action: "/NguyenPhuLieu/GhiNhanKiemKePhuLieu", IsFrm: false, isLink: true, contentType: 'external' },
    { ModuleID: "M.53.00.00", Action: "/PheDuyetNK/PheDuyetNK", IsFrm: false, isLink: true, contentType: 'external' },

    
];
var IsDuyetSelected = false;
var tabStates = {};

function saveTabState(moduleId, state) {
    tabStates[moduleId] = state;
}

function restoreTabState(moduleId) {
    return tabStates[moduleId] || null;
}

var currentModuleId = "overview";

function stripIndex(text) {
    if (!text) return "";
    return text.replace(/^[IVXLCDM]+\.\s*|^\d+\.\s*/g, '').trim();
}

function romanToInt(roman) {
    const map = { I: 1, V: 5, X: 10, L: 50, C: 100, D: 500, M: 1000 };
    let total = 0, prev = 0;
    for (let i = roman.length - 1; i >= 0; i--) {
        const curr = map[roman[i]];
        total += curr < prev ? -curr : curr;
        prev = curr;
    }
    return total;
}

function ConvertDate(strDate) {
    if (!strDate) return "";
    const d = new Date(strDate);


    const month = String(d.getMonth() + 1).padStart(2, '0'); // getMonth() trả 0-11
    const day = String(d.getDate()).padStart(2, '0');
    const year = d.getFullYear();

    return `${day}-${month}-${year}`;

}

function GetModuleName(ModuleID) {
    const objModule = modulesData.find(x => x.ModuleID == ModuleID)
    return !objModule ? "" : stripIndex(objModule.Title)
}

function toggleNode(element) {
    $(element).toggleClass('expanded');
    $(element).nextUntil('.parent').slideToggle(250);
}

$(document).on('click', '.sub-tab', function (e) {
    if ($(e.target).hasClass('close-tab')) {
        return;
    }
    $('.sub-tab').removeClass('active');
    $(this).addClass('active');

    const moduleId = $(this).data('module-id');
    currentModuleId = moduleId;
    SetupTabOverview(moduleId)
    if (moduleId == 'overview') {
        SetupTabOverview("overview");
    } else {
        loadModuleData(moduleId, IsDuyetSelected);
    }

    console.log('Sub-tab selected:', $(this).text().trim(), 'ModuleID:', moduleId);
})

function GetModule() {
    $.ajax({
        async: false,
        url: `/api/PheDuyet/Get?action=GetModule&para1=${UserID}`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            data.forEach(item => {
                item.CleanParentTitle = stripIndex(item.ParentTitle || '');
                item.CleanTitle = stripIndex(item.Title || '');
                item.ParentOrder = item.ParentTitle?.match(/^[IVXLCDM]+/)?.[0] || '';
                item.ChildOrder = item.Title?.match(/^\d+/)?.[0] || '';
            });
            data.sort((a, b) => {
                const parentA = romanToInt(a.ParentOrder);
                const parentB = romanToInt(b.ParentOrder);
                if (parentA !== parentB) return parentA - parentB;

                const childA = parseInt(a.ChildOrder);
                const childB = parseInt(b.ChildOrder);
                return childA - childB;
            });
            console.table(data);
            modulesData = data;
            FetchModuleTree(data);
        },
        error: function (err) {
            console.error('Error loading modules:', err);
        }
    });
}

function toggleSidebar() {
    const sidebar = $('.sidebar');
    const treeView = $('#treeView');

    if (sidebar.hasClass('mini-mode')) {
     
        sidebar.removeClass('mini-mode');
       
        if (sidebar.hasClass('collapsed')) {
            sidebar.removeClass('collapsed');
        }
    } else {
     
        sidebar.addClass('mini-mode');
        treeView.animate({ scrollTop: 0 }, 300);
    }
}

function toggleCollapse() {
    const sidebar = $('.sidebar');
    const treeView = $('#treeView');

    if (sidebar.hasClass('collapsed')) {
      
        sidebar.removeClass('collapsed');
    } else {
      
        treeView.animate({ scrollTop: 0 }, 300, function () {
            sidebar.addClass('collapsed');
        });
    }
  
}

function FetchModuleTree(modules) {
    const treeView = $('#treeView');
    treeView.empty();
    const groupedByModuleGroup = {};
    modules.forEach(mod => {
        if (!groupedByModuleGroup[mod.ModuleGroup]) {
            groupedByModuleGroup[mod.ModuleGroup] = [];
        }
        groupedByModuleGroup[mod.ModuleGroup].push(mod);
    });
    Object.keys(groupedByModuleGroup).forEach(groupName => {
        const groupModules = groupedByModuleGroup[groupName];
        const groupNode = $(`
            <div class="tree-item parent expanded" onclick="toggleNode(this)">        
                <i class="fas fa-cog"></i>
                <span>${groupName}</span>
            </div>
        `);
        treeView.append(groupNode);
        const groupedByParent = {};
        groupModules.forEach(mod => {
            const parentKey = mod.CleanParentTitle || 'No Parent';
            if (!groupedByParent[parentKey]) {
                groupedByParent[parentKey] = [];
            }
            groupedByParent[parentKey].push(mod);
        });
        Object.keys(groupedByParent).forEach(parentTitle => {
            const parentModules = groupedByParent[parentTitle];
            if (parentTitle !== 'No Parent') {
                const parentNode = $(`
                    <div class="tree-item child parent" onclick="toggleNode(this)">                       
                        <i class="fas fa-folder-open"></i>
                        <span>${parentTitle}</span>
                    </div>
                `);
                treeView.append(parentNode);
            }
            parentModules.forEach(mod => {
                let SoPhieu = 0;
                //const objSoPhieuCanDuyet = arrCountPhieuChuaDuyet.find(x => x.ModuleID == mod.ModuleID);
                //if (objSoPhieuCanDuyet != null) {
                //    SoPhieu = objSoPhieuCanDuyet.SoPhieu;
                //}
                const moduleNode = $(`
                    <div class="tree-item child-level-2" data-module-id="${mod.ModuleID}" data-title="${mod.CleanTitle}" data-is-frm="${mod.IsFrm}">
                        <i class="fa-brands fa-codepen"></i>
                        <span>${mod.CleanTitle}</span>
                        ${SoPhieu == 0 || !SoPhieu ? "" : ` <span class="so-phieu-can-duyet">(${SoPhieu})</span>`}
                    </div>
                `);
                moduleNode.on('click', function () {
                    const moduleId = $(this).data('module-id');
                    const title = $(this).data('title');
                    renderSubTab(moduleId, title, mod.IsFrm);
                    $('.tree-item').removeClass('active');
                    $(this).addClass('active');
                });
                treeView.append(moduleNode);
            });
        });
    });
}

function loadModuleData(moduleId, isDuyet) {
    const loaderWrapper = document.getElementById('customLoaderWrapper');
    const progressBar = document.getElementById('customProgressBar');

    if (!loaderWrapper || !progressBar) {
        loaderWrapper.classList.remove('active');
        progressBar.style.width = '0%';
        progressBar.setAttribute('data-percentage', '100%');
        return;
    }

    loaderWrapper.classList.add('active');
    progressBar.style.width = '0%';
    progressBar.setAttribute('data-percentage', '0%');

    let progress = 0;
    const interval = setInterval(() => {
        if (progress < 90) {
            progress++;
            progressBar.style.width = progress + '%';
            progressBar.setAttribute('data-percentage', progress + '%');
        }
    }, 30);
    if (moduleId == "M.04.02.00")
    {
        FetchBOM_TQ(isDuyet);
        clearInterval(interval);
        progressBar.style.width = '100%';
        progressBar.setAttribute('data-percentage', '100%');
        setTimeout(() => {
            loaderWrapper.classList.remove('active');
            progressBar.style.width = '0%';
            progressBar.setAttribute('data-percentage', '0%');
        }, 800);
        return
    }
    else {
        const actionObj = arrAction.find(item => item.ModuleID === moduleId);

        if (!actionObj) {
            loaderWrapper.classList.remove('active');
            progressBar.style.width = '0%';
            progressBar.setAttribute('data-percentage', '100%');
            SetGridview([]);
            return;
        }
        SetupTabOverview(moduleId)
        const action = actionObj.Action;


        $(".dx-toolbar-items-container").parents().each(function () {
            if ($(this).css("display") === "none") {
                $(this).css("display", "block");
            }
        });

        if (actionObj.isLink == true) {
            $(".group-btn-duyet").hide();
            loadExternalPageContent(action, interval, progressBar, loaderWrapper);
            return;
        }
        var apiUrl = ``;
        if (action == "GetPhieuCanDuyet") {
            moduleId = "overview";
        }
        else if (action == "GetDetailDH") {
            apiUrl = `/api/PheDuyet/Get?action=${action}&para1=ALL&para2=ALL`;

        }
        else {
            apiUrl = `/api/PheDuyet/Get?action=${action}&para1=${UserID}&para2=${!isDuyet ? 0 : 1}`;
        }
        fetch(apiUrl, { method: "GET" })
            .then(response => {
                if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
                return response.json();
            })
            .then(data => {
                console.log('Module data loaded:', data);
                var dataWithIndex = data.map((item, index) => ({
                    ...item,
                    rowIndex: index + 1
                })).sort((a, b) => a.rowIndex - b.rowIndex);
                SetGridview(dataWithIndex, moduleId);
            })
            .catch(error => {
                clearInterval(interval);
                progressBar.style.width = '100%';
                progressBar.setAttribute('data-percentage', '100%');
                setTimeout(() => {
                    loaderWrapper.classList.remove('active');
                    progressBar.style.width = '0%';
                    progressBar.setAttribute('data-percentage', '0%');
                }, 800);
                console.error('Error loading module data:', error);
                SetGridview([]);
            })
            .finally(() => {
                clearInterval(interval);
                progressBar.style.width = '100%';
                progressBar.setAttribute('data-percentage', '100%');
                setTimeout(() => {
                    loaderWrapper.classList.remove('active');
                    progressBar.style.width = '0%';
                    progressBar.setAttribute('data-percentage', '0%');
                }, 800);
            });
    }

   
}

function loadExternalPageContent(url, interval, progressBar, loaderWrapper) {
    const gridContainer = document.getElementById('gridContainer');
    if (gridContainer) {
        gridContainer.style.display = 'none';
    }

    let externalContainer = document.getElementById('externalPageContainer');
    if (!externalContainer) {
        externalContainer = document.createElement('div');
        externalContainer.id = 'externalPageContainer';
        externalContainer.style.cssText = 'width: 100%; height: 100%; overflow: auto;';
        document.querySelector('.main-content').appendChild(externalContainer);
    }
    externalContainer.style.display = 'block';

    fetch(url)
        .then(response => {
            if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
            return response.text();
        })
        .then(html => {
            const parser = new DOMParser();
            const doc = parser.parseFromString(html, 'text/html');

            let mainContent = doc.querySelector('.main-content, .content, #content, body');
            if (!mainContent) {
                mainContent = doc.body;
            }

            
            const cleanContent = mainContent.cloneNode(true);

    
            cleanContent.querySelectorAll('script').forEach(s => s.remove());

        
            cleanContent.querySelectorAll('link[rel="stylesheet"]').forEach(l => l.remove());

     
            cleanContent.querySelectorAll('style').forEach(s => s.remove());

          
            externalContainer.innerHTML = cleanContent.innerHTML;

         
            loadFilteredStyles(doc);

          
            setTimeout(() => {
                loadFilteredScripts(doc, externalContainer);
            }, 100);

            console.log('External page loaded successfully (CSS & scripts filtered):', url);
        })
        .catch(error => {
            console.error('Error loading external page:', error);
            externalContainer.innerHTML = `
                <div style="padding: 20px; text-align: center;">
                    <i class="fas fa-exclamation-triangle" style="font-size: 48px; color: #ff6b6b;"></i>
                    <h3>Không thể tải nội dung</h3>
                    <p>Vui lòng thử lại sau hoặc liên hệ quản trị viên.</p>
                </div>
            `;
        })
        .finally(() => {
            clearInterval(interval);
            progressBar.style.width = '100%';
            progressBar.setAttribute('data-percentage', '100%');
            setTimeout(() => {
                loaderWrapper.classList.remove('active');
                progressBar.style.width = '0%';
                progressBar.setAttribute('data-percentage', '100%');
            }, 800);
        });
}

function loadFilteredStyles(doc) {
    const blockedStyles = [
        'bootstrap.min.css',
        'bootstrap.css',
        'dx.common.css',
        'dx.light.css',
        'dx.dark.css',
        'devextreme',
        'fontawesome',
        'font-awesome'
    ];

    // ========== LOAD EXTERNAL STYLESHEETS ==========
    const links = Array.from(doc.querySelectorAll('link[rel="stylesheet"]'));
    links.forEach(link => {
        const href = link.href || '';

        const isBlocked = blockedStyles.some(blocked =>
            href.toLowerCase().includes(blocked.toLowerCase())
        );

        if (isBlocked) {
            console.log('Skipped CSS library:', href);
            return;
        }

        const existingLink = document.querySelector(`link[href="${href}"]`);
        if (existingLink) {
            console.log('CSS already loaded:', href);
            return;
        }

        const newLink = document.createElement('link');
        newLink.rel = 'stylesheet';
        newLink.href = href;

        Array.from(link.attributes).forEach(attr => {
            if (attr.name !== 'rel' && attr.name !== 'href') {
                newLink.setAttribute(attr.name, attr.value);
            }
        });

        document.head.appendChild(newLink);
        console.log('Loaded CSS:', href);
    });

    // ========== LOAD INLINE STYLES ==========
    const styles = Array.from(doc.querySelectorAll('style'));
    styles.forEach(style => {
        const content = style.textContent || '';

        const isLargeLibrary =
            (content.includes('@import') && blockedStyles.some(lib =>
                content.toLowerCase().includes(lib.toLowerCase())
            )) ||
            (content.length > 50000 && ( // Chỉ block file CSS lớn chứa library
                content.includes('devextreme') ||
                content.includes('bootstrap') ||
                content.includes('font-awesome')
            ));

        if (isLargeLibrary) {
            console.log('Skipped large library inline style');
            return;
        }

        // LOAD TẤT CẢ inline styles còn lại (bao gồm custom styles của trang)
        const newStyle = document.createElement('style');
        newStyle.textContent = content;

        Array.from(style.attributes).forEach(attr => {
            newStyle.setAttribute(attr.name, attr.value);
        });

        document.head.appendChild(newStyle);
        console.log('Loaded inline style');
    });
}

function loadFilteredScripts(doc, container) {
    const blockedLibraries = [
        'jquery.min.js',
        'jquery.js',
        'dx.all.js',
        'dx.all.min.js',
        'devextreme',
        'bootstrap.bundle',
        'bootstrap.min.js',
        'fontawesome'
    ];

    const scripts = Array.from(doc.querySelectorAll('script'));

    // Tách inline và external scripts
    const inlineScripts = [];
    const externalScripts = [];

    scripts.forEach(script => {
        const src = script.src || '';
        const content = script.textContent || '';

        if (src) {
            // External script
            const isBlocked = blockedLibraries.some(lib =>
                src.toLowerCase().includes(lib.toLowerCase())
            );
            if (!isBlocked) {
                externalScripts.push(script);
            } else {
                console.log('Skipped script library:', src);
            }
        } else if (content.trim()) {
            // Inline script - CHỈ block nếu import/require library bị cấm
            const hasBlockedImport =
                (content.includes('import') || content.includes('require')) &&
                blockedLibraries.some(lib =>
                    content.toLowerCase().includes(lib.toLowerCase())
                );

            if (!hasBlockedImport) {
                inlineScripts.push(script);
            } else {
                console.log('Skipped inline script with blocked import');
            }
        }
    });

    // Load inline scripts TRƯỚC để định nghĩa biến/hàm global
    const scriptsToLoad = [...inlineScripts, ...externalScripts];

    console.log(`Loading ${inlineScripts.length} inline scripts and ${externalScripts.length} external scripts`);
    loadScriptsSequentially(scriptsToLoad, 0);
}

function loadScriptsSequentially(scripts, index) {
    if (index >= scripts.length) {
        console.log('All scripts loaded');

      
        if (typeof window.initPageContent === 'function') {
            try {
                window.initPageContent();
            } catch (e) {
                console.error('Error initializing page:', e);
            }
        }
        return;
    }

    const oldScript = scripts[index];
    const newScript = document.createElement('script');


    Array.from(oldScript.attributes).forEach(attr => {
        newScript.setAttribute(attr.name, attr.value);
    });

    if (oldScript.src) {
   
        const src = oldScript.src;

        if (document.querySelector(`script[src="${src}"]`)) {
            console.log('Script already loaded:', src);
            loadScriptsSequentially(scripts, index + 1);
            return;
        }

        newScript.onload = () => {
            console.log('Loaded script:', src);
            loadScriptsSequentially(scripts, index + 1);
        };

        newScript.onerror = () => {
            console.error('Failed to load script:', src);
            loadScriptsSequentially(scripts, index + 1);
        };

        document.body.appendChild(newScript);
    } else {
  
        newScript.textContent = `
            (function() {
                try {
                    ${oldScript.textContent}
                } catch(e) {
                    console.error('Script execution error:', e);
                }
            })();
        `;

        document.body.appendChild(newScript);

  
        setTimeout(() => loadScriptsSequentially(scripts, index + 1), 10);
    }
}

function SetGridview(data, moduleId) {
    const gridContainer = document.getElementById('gridContainer');
    const externalContainer = document.getElementById('externalPageContainer');

    // Hiện grid, ẩn external content
    if (gridContainer) {
        gridContainer.style.display = 'block';
    }

    if (externalContainer) {
        externalContainer.style.display = 'none';
    }

    const gridInstance = $("#gridContainer").dxDataGrid("instance");
    if (!gridInstance) {
        console.error("DataGrid instance not found!");
        return;
    }
    else if (gridInstance) {

        if (data && data.length > 0) {
            const columns = generateColumns(data[0], moduleId);
            gridInstance.option("columns", columns);
        }

        gridInstance.option("dataSource", data);

        gridInstance.option("customModuleId", moduleId);

        const toolbarConfig = generateToolbar(moduleId);
        gridInstance.option("toolbar", {
            items: toolbarConfig
        });
        gridInstance.refresh();
    }
}

function renderDefaultTab() {
    var IsHasOverview = modulesData.filter(x => x.ModuleID != "M.24.00.00" && x.ModuleID != "M.25.00.00").length > 0;
    if (modulesData.length == 1 && modulesData[0].ModuleID == "M.04.02.00" && IsHasOverview) {
        currentModuleId = modulesData[0].ModuleID;
        /*renderedTabs.push(modulesData[0].ModuleID);*/
        renderSubTab(modulesData[0].ModuleID, stripIndex(modulesData[0].Title || ''), modulesData[0].IsFrm)

    }
    else if (IsHasOverview) {
        const overviewTab = $(`
        <div class="sub-tab active" data-module-id="overview">
          <i class="fa-brands fa-google-drive"></i>
            <span>Overview</span>
        </div>
    `);
        $('.sub-tabs-container').append(overviewTab);
     
        SetupTabOverview('overview')
        renderedTabs.push('overview');
        currentModuleId = 'overview';

        
    }
    else
    {

        const waitTreeRender = setInterval(() => {
            const element = $("#treeView .tree-item.child-level-2").first();

            if (element.length) {
                clearInterval(waitTreeRender);
                handleClick(element);
            }
        }, 50);

        function handleClick(element) {
            const moduleId = element.attr("data-module-id");
            const title = element.attr("data-title");
            const isFrm = element.attr("data-is-frm") === "true";

            element.trigger("click");
            renderSubTab(moduleId, title, isFrm);

            $(".tree-item").removeClass("active");
            element.addClass("active");
        }



    }

}

function renderSubTab(moduleId, title, IsFrm) {
    const isRendered = renderedTabs.includes(moduleId);
    if (!isRendered) {
        renderedTabs.push(moduleId);
        const subTab = $(`
            <div class="sub-tab" data-module-id="${moduleId}" data-isfrm="${IsFrm}">
                <i class="fa-brands fa-codepen"></i>
                <span>${title}</span>
                <span class="close-tab" onclick="closeSubTab('${moduleId}','${IsFrm}', event)">x</span>
            </div>
        `);
        $('.sub-tabs-container').append(subTab.hide().fadeIn(300));
    }

    $('.sub-tab').removeClass('active');
    $(`.sub-tab[data-module-id="${moduleId}"]`).addClass('active');
    currentModuleId = moduleId;

    loadModuleData(moduleId, IsDuyetSelected);

    if (isRendered) $("#btn-duyet-all").show();
}

function loadExternalPageContentIframe(url, interval, progressBar, loaderWrapper) {
    // Ẩn grid container
    const gridContainer = document.getElementById('gridContainer');
    if (gridContainer) {
        gridContainer.style.display = 'none';
    }

    // Tạo hoặc lấy iframe container
    let iframeContainer = document.getElementById('iframeContainer');
    if (!iframeContainer) {
        iframeContainer = document.createElement('div');
        iframeContainer.id = 'iframeContainer';
        iframeContainer.style.cssText = 'width: 100%; height: calc(100vh - 150px); position: relative;';

        const iframe = document.createElement('iframe');
        iframe.id = 'externalPageIframe';
        iframe.style.cssText = 'width: 100%; height: 100%; border: none; border-radius: 4px;';

        iframeContainer.appendChild(iframe);
        document.querySelector('.main-content').appendChild(iframeContainer);
    }

    iframeContainer.style.display = 'block';
    const iframe = document.getElementById('externalPageIframe');

    // Load URL vào iframe
    iframe.onload = function () {
        clearInterval(interval);
        progressBar.style.width = '100%';
        progressBar.setAttribute('data-percentage', '100%');

        setTimeout(() => {
            loaderWrapper.classList.remove('active');
            progressBar.style.width = '0%';
            progressBar.setAttribute('data-percentage', '100%');
        }, 800);
    };

    iframe.onerror = function () {
        clearInterval(interval);
        loaderWrapper.classList.remove('active');
        DevExpress.ui.notify("Không thể tải trang", 'error', 2000);
    };

    iframe.src = url;
}

function closeSubTab(moduleId, IsFrm, event) {
    event.stopPropagation();
    if (moduleId === 'overview') {
        return;
    }

    renderedTabs = renderedTabs.filter(id => id !== moduleId);

    const tab = $(`.sub-tab[data-module-id="${moduleId}"]`);
    tab.fadeOut(200, function () {
        $(this).remove();

        if ($('.sub-tab').length > 0) {
            const lastTab = $('.sub-tab').last();
            lastTab.addClass('active');
            currentModuleId = lastTab.data('module-id');

            if (currentModuleId === 'overview') {
                // Ẩn external content khi quay về overview
                const externalContainer = document.getElementById('externalPageContainer');
                const iframeContainer = document.getElementById('iframeContainer');
                if (externalContainer) externalContainer.style.display = 'none';
                if (iframeContainer) iframeContainer.style.display = 'none';

                SetupTabOverview("overview");
            } else {
                loadModuleData(currentModuleId, IsDuyetSelected);
            }
        }
    });
}

function loadOverviewData() {
    const loaderWrapper = document.getElementById('customLoaderWrapper');
    const progressBar = document.getElementById('customProgressBar');

    if (!loaderWrapper || !progressBar) {
        console.error("Thiếu phần tử customLoaderWrapper hoặc customProgressBar.");
        return;
    }

    // Hiện loader
    loaderWrapper.classList.add('active');
    progressBar.style.width = '0%';
    progressBar.setAttribute('data-percentage', '0%');

    let progress = 0;
    const interval = setInterval(() => {
        if (progress < 90) {
            progress++;
            progressBar.style.width = progress + '%';
            progressBar.setAttribute('data-percentage', progress + '%');
        }
    }, 30);
    if (!UserID) {
        UserID= localStorage.getItem("UserID1")
    }
    const apiUrl = `/api/PheDuyet/Get?action=GetPhieuCanDuyet&para1=${UserID}&para2=${IsDuyetSelected}`;

    fetch(apiUrl, { method: "GET" })
        .then(response => {
            if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
            return response.json();
        })
        .then(data => {

            SetGridview(data, "overview");

        })
        .catch(error => {
            DevExpress.ui.notify("Lỗi Kết Nối Mạng . Vui Lòng Thử Lại Sau", 'warning', 2000);
            SetGridview([]);
        })
        .finally(() => {
            clearInterval(interval);
            progressBar.style.width = '100%';
            progressBar.setAttribute('data-percentage', '100%');

            setTimeout(() => {
                loaderWrapper.classList.remove('active');
                progressBar.style.width = '0%';
                progressBar.setAttribute('data-percentage', '100%');
            }, 800); // hiệu ứng fade-out
        });

}

function GetCountPhieuCanDuyet() {
    $.ajax({
       
        url: `/api/PheDuyet/Get?action=GetCountPhieuCanDuyet&para1=${UserID}`,
        contentType: 'application/json;charset=utf-8',
        success: function (data) {
            arrCountPhieuChuaDuyet = [...data]
        },
        error: function (err) {
            console.error('Error loading modules:', err);
        }
    });
}

async function loadDataFromAPI(apiUrl) {
    try {
        const response = await fetch(apiUrl);

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        console.log('Data loaded:', data);

        return data;
    } catch (error) {
        console.error('Error loading data:', error);
        throw error;
    }
}

function fetchTabLSXDonHang() {
    const loaderWrapper = document.getElementById('customLoaderWrapper');
    const progressBar = document.getElementById('customProgressBar');

    if (!loaderWrapper || !progressBar) {
        loaderWrapper.classList.remove('active');
        progressBar.style.width = '0%';
        progressBar.setAttribute('data-percentage', '100%');
        return;
    }

    // Hiện loader
    loaderWrapper.classList.add('active');
    progressBar.style.width = '0%';
    progressBar.setAttribute('data-percentage', '0%');

    let progress = 0;
    const interval = setInterval(() => {
        if (progress < 90) {
            progress++;
            progressBar.style.width = progress + '%';
            progressBar.setAttribute('data-percentage', progress + '%');
        }
    }, 30);

    var action = "GetDonHangTong"
    const apiUrl = `/api/PheDuyet/Get?action=${action}`;
    loadDataFromAPI(apiUrl)
        .then(data => {
            var dataWithIndex = data.map((item, index) => ({
                ...item,
                rowIndex: index + 1  // Bắt đầu từ 1
            })).sort((a, b) => a.rowIndex - b.rowIndex);
            $("#gridContainer").dxDataGrid({
                dataSource: dataWithIndex,            
                columns: [
                    {
                        dataField: "rowIndex",
                        caption: "STT",
                        width: 50,
                        alignment: "center",
                        allowFiltering: false,
                        allowHeaderFiltering: false

                    },
                    { dataField: "MaDH", caption: "Đơn hàng", width: 100 },
                    { dataField: "MaLenh", caption: "Mã lệnh", width: 90, cssClass: "col-soluong"},
                    { dataField: "TenHang", caption: "Tên hàng"},
                    { dataField: "TenCL", caption: "Chủng loại", width: 120},
                    { dataField: "TenKH", caption: "Khách hàng"},
                    { dataField: "NhanVien", caption: "Người tạo" },
                    {
                        dataField: "NgayTao",
                        caption: "Ngày tạo",
                        dataType: "date",
                        width: 110,
                        format: "dd/MM/yyyy"
                    },
                    { dataField: "Dot", caption: "SeaSon", width: 100 },
                    { dataField: "BookingMaHang", caption: "Số Booking", width: 120 },
                    {
                        dataField: "SoLuong",
                        caption: "Số lượng",
                        width: 100,
                        alignment: "right",
                        format: "#,##0",
                        cssClass: "col-soluong"
                    },
                    { dataField: "GhiChu", caption: "Ghi chú" },
                    {
                        dataField: "Status_LSX",
                        caption: "Phân loại",
                        minWidth: 80, width: 80,
                        allowFiltering: false,
                        cellTemplate: function (container, options) {
                            const status = options.value;
                            let color = "";

                            if (status === "Sản xuất") color = "#28a745";
                            else if (status === "Duyệt lại") color = "#17a2b8";
                            else if (status === "Gia công") color = "#ffc107";

                            $("<div>")
                                .css({
                                    "padding": "5px 10px",
                                    "background-color": color,
                                    "color": "white",
                                    "border-radius": "4px",
                                    "text-align": "center",
                                    "font-weight": "500",
                                    "font-size": "12px"
                                })
                                .text(status || "")
                                .appendTo(container);
                        }
                    },
                    {
                        caption: " ",
                        allowFiltering: false,
                        allowSorting: false,
                        minWidth: 110,
                        width: 110,
                        alignment: "center",
                        cellTemplate: function (container, options) {
                            $("<span>")
                                .html('<i class="fa-sharp fa-solid fa-eye"></i> <em style="color:#00008B; text-decoration:underline;">Xem chi tiết</em>')
                                .css("cursor", "pointer")
                                .on("click", function () {
                                    ShowDetailDHMaLenh(options.data.MaDH, options.data.MaLenh, options.data.IsDuyetLai);
                                })
                                .appendTo(container);
                        }
                    }
                   
                ],
                showBorders: true,
                showRowLines: true,
                rowAlternationEnabled: true,
                columnAutoWidth: true,
                selection: { mode: "single" },
                hoverStateEnabled: false,
                paging: {
                    enabled: false,

                },                   
                filterRow: { visible: true },
                headerFilter: { visible: true },            
                scrolling: {
                    mode: 'standard',
                    useNative: true,
                    showScrollbar: 'always',
                },
              
                             
            });
        }).catch(error => {
            clearInterval(interval);
            progressBar.style.width = '100%';
            progressBar.setAttribute('data-percentage', '100%');
            setTimeout(() => {
                loaderWrapper.classList.remove('active');
                progressBar.style.width = '0%';
                progressBar.setAttribute('data-percentage', '0%');
            }, 800);
            $("#gridDetailContainer").empty();
            $("#gridContainer").dxDataGrid({ dataSource: [] });

        }).finally(() => {
            clearInterval(interval);
            progressBar.style.width = '100%';
            progressBar.setAttribute('data-percentage', '100%');
            setTimeout(() => {
                loaderWrapper.classList.remove('active');
                progressBar.style.width = '0%';
                progressBar.setAttribute('data-percentage', '0%');
            }, 800);
        });
       
}

var selectedTab = 0;

function SetupTabOverview(moduleId) {
    var isVisibleDonHangTong = modulesData.findIndex(x => x.ModuleID == "M.04.01.00") !== -1;
    var isVisibleDuyetPhieu = modulesData.findIndex(x => x.ModuleID != "M.04.01.00") !== -1
    const allTabs = [
        { id: 1, text: "Duyệt lệnh sản xuất", visible: isVisibleDonHangTong},
        { id: 0, text: "Duyệt phiếu", visible: isVisibleDuyetPhieu},
       
    ];

    const isOverview = moduleId == 'overview';
   
    $("#dxMainTabs").dxTabs({
        dataSource: isOverview ? allTabs : [allTabs[0]], // chỉ tab 1
        selectedIndex: selectedTab ?? 0,
        visible: isOverview, // ẩn header khi không phải overview
        onItemClick: function (e) {
            selectedTab = e.itemData.id;
            const gridInstance = $("#gridContainer").dxDataGrid("instance");
            const toolbarConfig = generateToolbar(moduleId);
            gridInstance.option("toolbar", {
                items: toolbarConfig
            })
             if (selectedTab === 0) {
                $("#gridDetailContainer").hide();
                $(".infor-gridDetailContainer").hide();
                loadOverviewData();
            } else if (selectedTab === 1) {
                $("#gridDetailContainer").show();
                $(".infor-gridDetailContainer").show();
                fetchTabLSXDonHang();
            }
        }
    });

    if (moduleId == "overview") {
        if (isVisibleDonHangTong && !isVisibleDuyetPhieu ) {

            fetchTabLSXDonHang();
        }
       else if (selectedTab === 0) {      
            loadOverviewData();
        }
        else if (selectedTab === 1) {
           
            fetchTabLSXDonHang();
        }
    }
    if ((moduleId == "overview" && selectedTab == 1)) {
        $("#gridDetailContainer").show();

        $(".infor-gridDetailContainer").show();
    }
    else if (moduleId == "M.04.01.00") {
        $("#gridDetailContainer").show();
        $(".infor-gridDetailContainer").show();
    
    }
    else {
        $("#gridDetailContainer").hide();
        $(".infor-gridDetailContainer").hide();

    }

    
}

function ShowDetailDHMaLenh(maDH, MaLenh, IsDuyetLai) {

    let detailPopup = $("#popupDHPO").dxPopup({
        visible: false,
        title: "Chi tiết đơn hàng",
        width: "95vw",
        height: "95vh",
        showCloseButton: true,
        dragEnabled: false,
        hideOnOutsideClick: true,
        shading: true,
        shadingColor: "rgba(0,0,0,0.5)",
        wrapperAttr: {
            class: "dx-popup"
        },
        toolbarItems: [
            {
                widget: "dxButton",
                toolbar: "top",
                location: "after",
                options: {
                    text: "Duyệt",
                    type: "success",
                    icon: "check",
                    onClick: function () {
                        var grid = $("#gridLenhSX").dxDataGrid("instance");
                        var dataSource = grid.getDataSource();
                        var data = dataSource.items();
                        var MaLenh = data[0]["MaLenh"];
                        var maDH = data[0]["MaDH"];
                        DongBoLenhSX(maDH, MaLenh).then(result => {
                            if (!result) return;

                            var detail = `Đơn Hàng: ${data[0].MaDH || ''}\nMã Hàng: ${data[0].TenHang || ''}\nLệnh: ${data[0].MaLenh || ''}\nSố lượng: ${data[0].SoLuong || ''}`;
                            var title = IsDuyetLai == 1 ? "Lệnh sản xuất được ban hành lại" : "Lệnh sản xuất đã được ban hành";
                            var sendto = "ALL";
                            sendNotify("M.04.01.00", title, detail, sendto);

                            
                        });
                        
                        if (detailPopup) {
                            detailPopup.hide();
                        }
                    }
                }
            },
           
        ]
      
    }).dxPopup("instance");

    loadDataFromAPI(`/api/PheDuyet/Get?action=GetDetailDH&para1=${maDH}&para2=${MaLenh}`)
        .then(headerData => {
            if (headerData && headerData.length > 0) {
                const data = headerData[0];
                $("#headerInfo").html(`
                    <div><strong>KHÁCH HÀNG/BUYER:</strong> ${data.TenKH || ''}</div>                 
                    <div><strong>ĐƠN HÀNG:</strong> ${data.MaDH || ''}</div>                
                    <div><strong>CHỦNG LOẠI:</strong> ${data.TenCL || ''}</div>               
                    <div><strong>MÙA/SEASON:</strong> ${data.Dot || ''}</div>                  
                    <div><strong>TOTAL:</strong> ${data.SoLuong || ''}</div>
                    <div><strong>GHI CHÚ:</strong> ${data.GhiChu || ''}</div>
                `);

                detailPopup.option("title", `Mã Lệnh : ${MaLenh} - Mã hàng : ${data.TenHang || ''}`);
            }
        })
        .catch(error => {
            console.error('Error loading header:', error);
            DevExpress.ui.notify("Không thể tải thông tin đơn hàng", "error", 3000);
        });

  
    loadDataFromAPI(`/api/PheDuyet/Get?action=GetLenhSXDH&para1=${maDH}&para2=${MaLenh}`)
        .then(data => {
            let sizeColumns = [];
            if (data && data.length > 0) {
                const firstRow = data[0];
                Object.keys(firstRow).forEach(key => {
                    if (key.includes('@') && key !== 'Size') {
                        const sizeName = key.split('@')[0];
                        sizeColumns.push({
                            dataField: key,
                            caption: sizeName,
                            width: 90,
                            alignment: "center",
                            format: "#,##0",
                            cssClass: "col-soluong"
                        });
                    }
                });
            }

            $("#gridLenhSX").dxDataGrid({
                dataSource: data,
                columns: [
                    { dataField: "MaLenh", caption: "Mã lệnh", width: 100, visible: false },
                    { dataField: "PO", caption: "PO"},
                    { dataField: "DauSize", caption: "InSeam", width: 100 },
                    { dataField: "TenMau", caption: "Màu",  },
                    { dataField: "CodeMau", caption: "Code Màu",  },
                    { dataField: "Name", caption: "Chuyền", width: 100 },

                    ...sizeColumns,
                    {
                        caption: "Tổng",
                        width: 100,
                        alignment: "right",
                        format: "#,##0",
                        cssClass: "grand-total-cell",
                        summaryType: "sum",
                        calculateCellValue: function (rowData) {
                           
                            let total = 0;
                            sizeColumns.forEach(col => {
                                const value = rowData[col.dataField];
                                if (value && !isNaN(value)) {
                                    total += parseFloat(value);
                                }
                            });
                            return total;
                        }
                    }
                ],
                showBorders: true,
                showRowLines: true,
                rowAlternationEnabled: true,
                columnAutoWidth: true,
                hoverStateEnabled: true,
                searchPanel: {
                    visible: true,
                    placeholder: "Tìm kiếm...",
                    width: 250
                },
                headerFilter: { visible: false },
                filterRow: { visible: false },
                scrolling: {
                    mode: 'standard',
                    useNative: true,
                    showScrollbar: 'always',
                },
                summary: {
                    totalItems: [
                        {
                            column: "PO",
                            summaryType: "count",
                            customizeText: function (data) {
                                return "Grand Total";
                            }
                        },
                        ...sizeColumns.map(col => ({
                            column: col.dataField,
                            summaryType: "sum",
                            valueFormat: "#,##0",
                            customizeText: function (data) {
                                return data.value ? data.value.toLocaleString("vi-VN") : "0";
                            }
                        })),
                        {                            
                            showInColumn: "Tổng", 
                            summaryType: "custom",
                            customizeText: function (data) {
                               
                                let grandTotal = 0;
                                const gridData = $("#gridLenhSX").dxDataGrid("instance").option("dataSource");

                                gridData.forEach(row => {
                                    sizeColumns.forEach(col => {
                                        const value = row[col.dataField];
                                        if (value && !isNaN(value)) {
                                            grandTotal += parseFloat(value);
                                        }
                                    });
                                });

                                return grandTotal.toLocaleString('vi-VN');
                            }
                        }
                    ]
                }
            });
        })
        .catch(error => {
            console.error('Error loading Lệnh SX:', error);
            $("#gridLenhSX").dxDataGrid({ dataSource: [] });
            DevExpress.ui.notify("Không thể tải lệnh sản xuất", "error", 3000);
        });


    loadDataFromAPI(`/api/PheDuyet/Get?action=GetCapPhatDH&para1=${maDH}&para2=${MaLenh}`)
        .then(data => {
       
            $("#gridCapPhat").dxDataGrid({
                dataSource: data,
                columns: [
                    { dataField: "STT", caption: "STT", width: 100, alignment: "center" },
                    { dataField: "POMua", caption: "POMua", width: 100, alignment: "center" },
                    {
               
                        dataField: "MaLenh", caption: "Mã Lệnh", visible: false
                     
                    },
                    {
                        dataField: "NPL", caption: "NPL", groupIndex: 1,
                        groupCellTemplate: function (cellElement, cellInfo) {
                            const items = cellInfo.data.items;
                            cellElement.html(`<span style="color: brown; font-weight: bold;">${cellInfo.data.key}</span>`);
                        },
                    },
                    {
                        dataField: "TenNhomChiTiet", caption: "Chủng loại chi tiết", visible: false
                        
                    },
                  
                 
                    { dataField: "MaVTGhep", caption: "Mã vật tư", alignment: "center" ,visible: false},
                    { dataField: "MaVT", caption: "Item Code"},
                    { dataField: "ChiTiet", caption: "Mô tả" },               
                  
                    { dataField: "MauVT", caption: "Màu VT"},
                    { dataField: "KhoVai", caption: "Khổ/Size", width: 80 },
                    { dataField: "TenDVVT", caption: "Đơn vị", width: 60 },
                    {
                        dataField: "SoLuong",
                        allowFiltering: false,
                        allowHeaderFiltering: false,
                        caption: "Số Lượng",
                        width: 100,
                        alignment: "center",
                        format: "#,##0.##"
                    },
                    {
                        dataField: "DMKH",
                        caption: "Định mức khách hàng",
                        allowFiltering: false,
                        allowHeaderFiltering: false,
                        width: 80,
                        alignment: "right",
                        format: "#,##0.####"
                    },
                    {
                        dataField: "CapPhatKH",
                        caption: "Cấp phát tạm tính",
                        width: 80,
                        alignment: "right",
                        format: "#,##0.####"
                    },
                    { dataField: "GhiChuPKT", caption: "Ghi chú PKT" },                           
                    {
                        dataField: "DinhMucHaoHut", caption: "%Hao hụt", width: 100, allowFiltering: false,
                        allowHeaderFiltering: false },
                    {
                        dataField: "DinhMuc", caption: "Định mức cấp phát", width: 100, allowFiltering: false,
                        allowHeaderFiltering: false},
                    {
                        dataField: "ThucXuat", caption: "Tổng cấp pháp", width: 100, allowFiltering: false,
                        allowHeaderFiltering: false},
                    {
                        dataField: "Status",
                        caption: "Trạng thái",
                        width: 120,
                        cellTemplate: function (container, options) {
                            const status = options.value;
                            let color = status === "Đã xác nhận" ? "#28a745" : "#6c757d";

                            $("<div>")
                                .css({
                                    "padding": "3px 8px",
                                    "background-color": color,
                                    "color": "white",
                                    "border-radius": "3px",
                                    "text-align": "center",
                                    "font-size": "11px"
                                })
                                .text(status || "")
                                .appendTo(container);
                        }
                    },
                    { dataField: "GhiChu", caption: "Ghi chú PKH"},
                
                ],
                showBorders: true,
                showRowLines: true,
                rowAlternationEnabled: true,
                columnAutoWidth: true,
                hoverStateEnabled: true,
                headerFilter: { visible: false },
                filterRow: { visible: true },
                scrolling: {
                    mode: 'standard',
                    useNative: true,
                    showScrollbar: 'always',
                },
                paging: {
                    enabled: false,

                },
                groupPanel: { visible: false },
                searchPanel: {
                    visible: true,
                    placeholder: "Tìm kiếm...",
                   
                }
            });
        })
        .catch(error => {
            console.error('Error loading Cấp Phát:', error);
            $("#gridCapPhat").dxDataGrid({ dataSource: [] });
            DevExpress.ui.notify("Không thể tải danh sách cấp phát", "error", 3000);
        });

 
    detailPopup.show();
}

async function DongBoLenhSX(maDH, MaLenh) {
    const dialogResult = await DevExpress.ui.dialog.confirm(
        `Bạn có chắc muốn kích hoạt sản xuất cho lệnh: ${MaLenh} này không?`,
        "Xác nhận kích hoạt lệnh"
    );
    if (!dialogResult) return false;

    const listKichHoat = [{
        ID: 0,
        MaLenhSanXuat: "",
        MaLenh: MaLenh,
        TenLenh: "",
        MaDH: maDH,
        MaDVSX: "",
        DotSX: "",
        MaQG: "",
        POID: UserID,
        PO: "",
        MaMau: "",
        DauSizeID: "",
        DauSize: "",
        SizeID: "",
        Size: "",
        SoLuong: 0,
        TrangThai: 0,
        STTLenh: 0,
        MaGop: "",
        POID_T: "",
        MaCu: "",
        Line: "",
        GhiChu: ""
    }];

    try {
       
        await $.ajax({
            type: "POST",
            url: `/api/ERPDonHangTong/Post?action=PostKichHoat&type=@TypeTable`,
            data: JSON.stringify(listKichHoat),
            contentType: "application/json"
        });

       
        const headerData = await loadDataFromAPI(
            `/api/PheDuyet/Get?action=GetTTLenhDuyet&para1=${maDH}&para2=${MaLenh}`
        );
        if (!headerData || headerData.length === 0) return false;

        const rowLenh = headerData[0];
        const makh = rowLenh["MaKH"] ?? "";
        const mahang = rowLenh["MaHang"] ?? "";
        const maLenhSX = rowLenh["MaLenhSanXuat"];
        
     
        const madot = await getMaDotAsync(makh, mahang);
        if (!madot && rowLenh["GiaCong"] == "1") {

            DevExpress.ui.notify(`Đã duyệt lệnh ${MaLenh} thành công`, "success", 2000);

            if (currentModuleId === 'overview') {
                SetupTabOverview("overview");
            } else {
                loadModuleData(currentModuleId, IsDuyetSelected);
            }
            reloadPhieuCount();
            return true;

        }

     
        await Promise.all([
            getTSAsync(makh, mahang, UserID, madot),
            postIsKeoVeAsync(maDH, maLenhSX)
        ]);

    
        DevExpress.ui.notify(`Đã duyệt lệnh ${MaLenh} thành công`, "success", 2000);

        if (currentModuleId === 'overview') {
            SetupTabOverview("overview");
        } else {
            loadModuleData(currentModuleId, IsDuyetSelected);
        }
        reloadPhieuCount();

        return true;

    } catch (err) {
        console.error("DongBoLenhSX error:", err);
        DevExpress.ui.notify("Có lỗi khi kích hoạt", "error", 2000);
        return false;
    }
}
function getMaDotAsync(makh, mahang) {
    return $.ajax({
        type: "GET",
        url: `/api/ERPDonHangTong/Get?Action=GETMAXDOT&para=${makh}&para2=${mahang}`
    }).then(res => res?.[0]?.MaDot || "");
}
function getTSAsync(makh, mahang, userName, madot) {
    return $.ajax({
        type: "GET",
        url: `/api/KhoiTaoBOMV1/Get?action=GET_TSNEW&para1=${makh}&para2=${mahang}&para6=${userName}&para7=${madot}`
    });
}
function postIsKeoVeAsync(maDH, maLenhSX) {
    return $.ajax({
        type: "GET",
        url: `/api/ERPDonHangTong/GetChuyen?action=PostIsKeoVe&para=${maDH}&para5=${maLenhSX}`
    });
}
$(function () {

    if (window.CefSharp)
    {
        $(".main-container").addClass('lay-out-cefShap');
        toggleSidebar()
    }
    else {
        $(".main-container").removeClass('lay-out-cefShap');
    }

    if (window.innerWidth <= 600) {
        toggleSidebar();
        toggleCollapse();
    }
});
