/**
Upload and download files

//1 download datagrid treegrid data  Take the data as $("grid").zdata().ajax();
$("grid").zfile("xls").begin().end().error().page().compress('rar').download();


//2 download url
$("down").zfile(url).download();

//3 Download the file package 
$("down").zfile(url[]).downlad();
{
    downloadType:   "generate url method", 
    dataGenerate: "#grid"
    dataUrls:[]
    dataMethod:"SysUser/GetFile"
    generateType: "xls,doc,pdf"
    generateTitle: "["key:icon,value:'icon'",""]"
    compressType:  "zip,rar,none"   
}
**/

(function ($) {
    $.zGetOptoins = function (target, name, defaults) {
        if (!target) return null;
        if (!$.data(target, name))
            $(target).addClass(name).data(name, $.extend(true, {}, defaults));
        return $.data(target, name);
    };

    $.fn.zfile = function () {
        if (this.length == 0) throw "Selector error";

        var that = this, defaults, current, objfn;
        if (arguments[0] == 'upload') {
            defaults = $.extend(true, {}, $.fn.zfile.upload.defaults.options);
            current = $.zGetOptoins(that[0], "zfile_upload", defaults);
            objfn = zfile_upload;
        }
        else {
            defaults = $.extend(true, {}, $.fn.zfile.download.defaults.options);
            current = $.zGetOptoins(that[0], "zfile_download", defaults);
            objfn = zfile_download;
        }

        var options = {
            $this: that
            , defaults: defaults
            , current: current
            , arg0: arguments[0]
        };

        return new objfn(options);
    };

    $.fn.zfile.download = {};
    $.fn.zfile.download.defaults = {
        ext: ['xls', 'xlsx', 'doc', 'docx', 'pdf', 'rar']
       , options: {
           downloadType: "generate" //"generate url method"
          , dataGenerate: ""
          , dataUrls: []
          , dataMethod: ""
          , generateType: "xls"
          , generateTitle: []
          , generateAll: true
          , compressType: "none"
          , begin: function () { }
          , error: function () { }
          , end: function () { }
       }
    };

    $.fn.zfile.upload = {};
    $.fn.zfile.upload.defaults = {
        options: {
            text: 'Upload'
          , listSelector: ''
          , type: 'import' //data import，Project attachment project，Temporary Files temp，Material picture picture...
          , billno: '' //business id
          , params: []
          , method: 'SysUser/import'
          , importParams: { map: [{ 'UserCode': 'username' }], keys: ['UserCode'], errorContinue: false }
          , progress: true
          , begin: function () { }
          , success: function () { }
          , error: function () { }
            //, progress: function () { }
        }
    };

})(jQuery);

var zfile_upload = function (options) {
    var defaults = {}, current = {}, arg0, $that, that = this;

    if (options) {
        $that = options.$this;
        defaults = options.defaults;
        current = options.current;
        arg0 = options.arg0;
    }

    that.text = function (text) {
        current.text = text;
        return that;
    };

    that.type = function (type) {
        current.type = type;
        return that;
    };

    that.method = function (method) {
        current.method = method;
        return that;
    };

    that.upload = function () {
        //        using('../common/js/uploader/fileuploader.js', function () {
        //            var uploader = new qq.FileUploader({
        //                element: $that[0],
        //                action: '/common/page/upload.aspx',
        //                uploadButtonText: 'Upload',
        //                onComplete: function (id, fileName, responseJSON) {
        //                    alert(responseJSON.success)
        //                },
        //                onProgress: function () { alert('onProgress') },
        //                onError: function (id, name, reason) { alert(reason); }
        //            });
        //        });

        using('messager', function () {
            //Add dynamicallykissy.js invalid，So it must be added automatically kiss.js Quote
            KISSY.config({ packages: [{ name: "gallery", path: "/Content/js/kissy/", charset: "utf-8" }] });
            KISSY.use('gallery/form/1.3/uploader/index', function (S, RenderUploader) {
                var ru = new RenderUploader('#' + $that.attr("id"), "#list", {
                    serverConfig: { action: "/Service/File/Upload", data: { test: '123' } },
                    type: 'auto',
                    name: "Filedata", // File domain
                    urlsInputName: "fileUrls"  //Used to put the server-side return url Hidden field
                });

                ru.on("init", function (ev) {
                    var uploader = ev.uploader;

                    uploader.on('render', function (ev) {
                        //alert('The upload component is ready！');
                    });
                    uploader.on('select', function (ev) {
                        var files = ev.files;
                        //alert('chosen' + files.length + 'A file');
                    });
                    uploader.on('start', function (ev) {
                        var index = ev.index, file = ev.file;
                        //alert ('start uploading, filename:' + file.name + ', queue index:' + index);
                        $.messager.progress({ title: '请稍等', msg: '正在上传...', interval: 0 });
                    });
                    uploader.on('progress', function (ev) {
                        var file = ev.file, loaded = ev.loaded, total = ev.total;
                        //alert ('being uploaded, file name:' + file.name + ', size:' + total + ', uploaded:' + loaded);
                        $.messager.progress('bar').progressbar('setValue', Math.ceil(loaded * 100 / total));
                        //if (loaded == total) $.messager.progress('close');
                    });
                    uploader.on('success', function (ev) {
                        var index = ev.index, file = ev.file;
                        //Server returns the result set
                        var result = ev.result;
                        //alert('Upload is successful, the server returns the upload mode：' + result.type);
                        //$.messager.progress('close')
                    });
                    uploader.on('complete', function (ev) {
                        var index = ev.index, file = ev.file;
                        //Server returns the result set
                        var result = ev.result;

                        $.messager.progress('close')
                        //alert('The upload ends, the server returns the upload status：' + result.status);
                    });
                    uploader.on('error', function (ev) {
                        var index = ev.index, file = ev.file;
                        //Server returns the result set
                        var result = ev.result;
                        alert('The upload failed with the error message：' + result.msg);
                    });
                    uploader.on('add', function (ev) {
                        var queue = ev.queue;
                        var file = ev.file;
                        //alert('Queue to add files! The file name is：' + file.name);
                    });
                    uploader.on('remove', function (ev) {
                        var queue = ev.queue;
                        //alert('Queue delete files! File index value：' + ev.index);
                        //alert('The number of files in the queue is：' + queue.get('files').length);
                    });
                });
            });
        });
    };
}

var zfile_download = function (options) {
    var defaults = {}, current = {}, arg0, $that, that = this;

    if (options) {
        $that = options.$this;
        defaults = options.defaults;
        current = options.current;
        arg0 = options.arg0;
    }

    //Export all pages without calling, page () without parameters current page, page (1,50) parameters corresponding page
    that.paging = function () {
        if (arguments.length == 0)
            current.dataGenerate.data.query.paging = $.extend(true, {}, $that.zdata().currentOptions().data.query.paging);
        else if (arguments.length == 2)
            current.dataGenerate.data.query.paging = { page: arguments[0], pagesize: arguments[1] };

        return that;
    };

    // .title('code','coding'); add to
    // .title([{key:'code',value:'coding'},{key:'xx',value:'yy'}],displayAll);
    that.title = function () {
        if (arguments[0] instanceof Array) {
            current.generateTitle = arguments[0];
            current.generateAll = (arguments[1] == true);
        } else if (typeof arguments[0] == 'string' && typeof arguments[1] == 'string') {
            current.generateTitle.push({ Key: arguments[0], Value: arguments[1] });
        }

        return that;
    };

    //Do not pass the default parameter is rar parameter parameters with the value of none rar zip 7z ...
    that.zip = that.compress = function () {
        current.compressType = "zip";
        return that;
    };

    //Unrealized event
    that.begin = function (fn) {
        if ($.isFunction(fn))
            current.begin = fn;
    }

    that.end = function (fn) {
        if ($.isFunction(fn))
            current.end = fn;
    }

    that.error = function (fn) {
        if ($.isFunction(fn))
            current.error = fn;
    }

    //Download processing
    that.download = function () {
        //Front desk inspection  //todo
        //if (_isDownloading) return;

        var params = $.extend(true, {}, current);
        params.dataGenerate = JSON.stringify(current.dataGenerate);
        params.generateTitle = JSON.stringify(current.generateTitle);
        params.dataUrls = JSON.stringify(params.dataUrls);

        //create iframe
        var downloadHelper = $('<iframe style="height: 0px; visibility: hidden;" id="downloadHelper"></iframe>').appendTo('body')[0];

        var doc = downloadHelper.contentWindow.document;
        if (doc) {
            doc.open();
            doc.write('')//Microsoft for doc.clear();
            doc.writeln('<html><body><form id="downloadForm" name="downloadForm" method="post" action="/Service/File/Download">');
            for (var key in params) doc.writeln(String.format("<input type='hidden' name='{0}' value='{1}'>", key, params[key]));
            doc.writeln('<\/form><\/body><\/html>');
            doc.close();
            var form = doc.forms[0];
            if (form) {
                current.begin();
                form.submit();
                this._isDownloading = true;
            }
        }
    };

    var init = function () {
        //init zfile
        var _isDownloading = false;
        if (arg0) current = defaults; //Only $ ("# grid"). Zfile () access, it returns the original settings

        if (arg0 instanceof Array) {  //Download package
            current.downloadType = "url";
            current.dataUrls = arg0;
        }
        else if (typeof arg0 == 'string') //Download a single file
        {
            if ($.inArray(arg0, $.fn.zfile.download.defaults.ext) > -1) { //Download screen list data
                //Chinese head set
                var dg = $that.data('datagrid');
                if (dg && dg.options && dg.options.columns) {
                    var titles = [];
                    $.each(dg.options.columns, function (i, item) { $.each(item, function (i, col) { if (!col.hidden) titles.push({ Key: col.field, Value: col.title }); }); });
                    that.title(titles);
                }
                current.downloadType = "generate";
                current.generateType = arg0;
                current.dataGenerate = $.extend(true, {}, $that.zdata().currentOptions()); //Default current zdata
                that.paging(1, 0); //Export all by default
            }
            else {
                current.downloadType = "url";
                current.dataUrls.push(arg0);;
            }
        }
        return that;
    }

    return init();

};
