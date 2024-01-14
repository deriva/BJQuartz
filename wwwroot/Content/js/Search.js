
var ban = new Ban({
    data: {
        Search: {
            Sql2: "",
            Alias: LP.GetQueryString("alias"),
            DbName: LP.GetQueryString("database"),
            TableName: LP.GetQueryString("tablename"),
            No: LP.GetQueryString("no"),
            Sql: "",
        }, //搜索条件
        SearchUrl: '/DataBase/ExecSql',
        LstConfig: [],
        Tables: [],
        H: 0,

        currnode: {}/*当前选中的节点*/
    },
    watchinfo: "P.watchinfo"
});
var tree;
var treeoptions;
var P = {
    watchinfo: (keyname, _key, value, pk) => {
        if (keyname == "Search.TableName") {
            // if ($.trim(ban.$data.Search.Sql).length == 0)
            // ban.$data.Search.Sql2 += " select * from  " + value + ";\r\n";

            var v = editor.getValue();
            v += " select * from  " + value + ";\r\n";
            //  else ban.$data.Search.Sql += "   " + value;
            editor.setValue(v);
        }

    },
    SetSql: (sql) => {//设置sql语句
        //   ban.$data.Search.Sql2 = sql;
        editor.setValue(sql);
    },
    SearchData: () => {//查询数据库
        var sql = editor.getValue();
        var dto = JSON.parse(JSON.stringify(ban.$data.Search));
        if (sql.length > 0) dto.Sql = sql;
        else
            dto.Sql = dto.Sql2;

        if (dto.length == 0) {
            LP.ToastError("请先输入sql");
            return;
        }
        $("#header1 tr").html("");
        $("#listtable").html("");



        var layindex = layer.msg('正在努力加载中......', {
            icon: 16,
            shade: 0.21
        });
        http.post(ban.$data.SearchUrl, dto).then(r => {
            layer.closeAll();
            if (r.code != 100)
                top.message.ToastCode(r);
            P.ReaderData(r);
        });
    },
    ReaderData: (r) => {
        if (r.code != 100) return;
        var lstR = [];
        for (var i = 0; i < r.attr.length; i++) {
            var lst01 = JSON.parse(r.attr[i]);
            var firstrow = lst01[0];
            var item = { columns: [], rows: [] };
            for (var it in firstrow) {
                item.columns.push({ title: it });
            }
            var ttll = lst01.length > 3000 ? 3000 : lst01.length;
            for (var j = 0; j < ttll; j++) {
                var rowdata = lst01[j];
                var rowdataArry = [];
                for (var it in firstrow) {
                    rowdataArry.push({ value: rowdata[it] });
                }
                item.rows.push({ data: rowdataArry });

            }
            lstR.push(item);
        }
        LP.TmplReader({ lst: lstR }, "tpl_tablistresult");
        layui.use('element', function () {
            var element = layui.element;
        });
        SqlEditor.setDatalistHeight();
        //  P.CalcHeight();
    },

    //新建查询
    OpenTab: () => {
        var url = "/page/database/sql/dbmain.html?v=" + (Math.random() * (15 - 10) + 10)
        LP.AddTab({
            href: url,
            title: "新建查询",
            tabId: url,
        })
    },
    OpenGroup: () => {
        var url = "/page/database/configinfo/group.html?";
        url = new dbhelper({ Search: ban.$data.Search }).GetUrlParm(url);
        var tabid = url;
        LP.AddTab({
            href: url,
            title: "关系图-" + ban.$data.Search.DbName,
            tabId: tabid,
        })
    },
    OpenTable: () => {//打开表管理
        var url = "/page/tableinfo/common/tableinfo.html?";
        url = new dbhelper({ Search: ban.$data.Search }).GetUrlParm(url);
        var tabid = "/page/tableinfo/common/tableinfo.html";
        LP.AddTab({
            href: url,
            title: "表管理-" + ban.$data.Search.DbName,
            tabId: tabid,
        })
    },
    //DDL
    OpenMyDb: () => {
        var url = "/page/database/configinfo/mydb.html?";
        url = new dbhelper({ Search: ban.$data.Search }).GetUrlParm(url);
        var tabid = url;
        LP.AddTab({
            href: url,
            title: "DDL-" + ban.$data.Search.DbName,
            tabId: tabid,
        })
    },
    TableData: () => {
        var url = "/page/database/configinfo/tabledata.html?";
        url = new dbhelper({ Search: ban.$data.Search }).GetUrlParm(url);
        var tabid = "/page/database/configinfo/tabledata.html";
        LP.AddTab({
            href: url,
            title: "表数据-" + ban.$data.Search.DbName,
            tabId: tabid,
        })
    },
    //选中表
    CheckTableName: (tablename) => {
        var sql = "select   * from " + tablename;
        // sql += "  limit 10";
        ban.$data.Search.Sql2 = sql;
        ban.$data.Search.TableName = tablename;
        //  P.SearchData();
    },
    //选中数据库
    CheckDataBase: (no, alias, dbname) => {
        ban.$data.Search.No = no;
        ban.$data.Search.Alias = alias;
        ban.$data.Search.DbName = dbname;
        P.GetTableName();
    },
    DialogDataBase: () => {//弹窗选择数据库
        LP.DialogRb("/page/database/configinfo/checkdb.html", 500, 220, "", 2, "auto")
    },
    Export: () => {
        var mggx = ban.$data.Search.TableName;
        let options = {
            title: '请填写导出名字:默认' + mggx + ".xls ",
            value: mggx
        };
        layer.prompt(options, function (value, index, elem) {
            if (!value) value = mggx;
            var tables = [];
            var tabs = document.querySelectorAll(".resulttable").forEach(x => {
                tables.push(x.innerHTML);
            });
            JSToXls.toExcels(tables, value);
            top.message.Ok("导出中....,请点击浏览器右上角的下载");
            layer.closeAll();
        });
    },
    ExportData: () => {
        new dbhelper({
            Search: ban.$data.Search
        }).ExportData(editor.getValue());
    },
   
    GetTableName: () => {
        new dbhelper({
            Search: ban.$data.Search,
            callback: (lst) => {
                let sel = document.querySelector("[name='TableName']"); var defval = sel.getAttribute("defval");
                sel.length = 0;
                ban.$data.Tables.length = 0;
                if (ban.$data.Search.TableName)
                    defval = ban.$data.Search.TableName;
                var tts = {};
                lst.forEach(x => {
                   
                    sel.options.add(new Option(x.Name, x.Name, defval == x.Name));
                    ban.$data.Tables.push(x.Name);
                    tts[x.Name] = ["Id"];
                });
                sel.setAttribute("defval", defval);
                LP.InitDefval();
                window.tablewords = tts; 
                //   P.AutoComplete();
            }
        }).GetTableRow("");

    },
    //获取所有表和字段
    GetAllTableColumn: () => {
        //new dbhelper({
        //    Search: ban.$data.Search,
        //    callback: (lst) => {
        //    //    window.tablewords = lst;
        //    }
        //}).GetAllTableColumn();
    },
    RegientEvent: () => {
        $(document).keydown(function (event) {
            var keycode = event.keyCode;
            var currkey = [112, 116, 27];
            switch (keycode) {
                case 112://F1
                    P.Export();
                    break;
                case 116://F5
                    P.SearchData();
                    break;
                case 27://esc
                    ban.$data.Search.Sql2 = " ";
                    break;
            }
            if (currkey.indexOf(keycode) > -1)
                event.preventDefault(); // 阻止默认行为
        });

    },
    //获取选中的文本
    GetSelectedText: function () {
        let selectedText = '';
        if (window.getSelection) { // 检查浏览器是否支持 window.getSelection()
            selectedText = window.getSelection().toString(); // 获取选中的文本并转换为字符串
        } else if (document.selection && document.selection.type !== 'Control') {
            selectedText = document.selection.createRange().text;
        }
        return selectedText;
    },
    ImportTable: () => {
        new dbhelper({
            Search: ban.$data.Search
        }).ImportTable();
    },
    Init: () => {
        var dbinfoStr = localStorage.getItem("checkdb");
        if (dbinfoStr && ban.$data.Search.No.length==0) {
            var dbinfo = JSON.parse(dbinfoStr);
            ban.$data.Search.Alias = dbinfo.Alias;
            ban.$data.Search.No = dbinfo.No;
            ban.$data.Search.DbName = dbinfo.DbName;
            P.GetTableName();

           // setTimeout(() => { P.GetAllTableColumn(); }, 1000);

        }
        P.RegientEvent();


    },
    DialogSql: (type) => {//弹窗查看sql
        var title = "常用sql";
        var url = "/page/database/sql/CommonExecSql.html";
        if (type == 1)//历史
        {
            url = "/page/database/sql/HistoryExecSql.html"; title = "历史sql";
        }
        url += "?v=" + (Math.random() * (15 - 10) + 10);
        top.LP.DialogRb(url, 0, 0, title, 2);
    },
    JoinCommonExecSql: () => {//加入常用sql语句
        var sql = editor.getValue();
        if (sql.length == 0) return false;
        var dto = {
            Sql: editor.getValue(),
            Title: "",
            DbName: ban.$data.Search.DbName,
            No: ban.$data.Search.No,
            Alias: ban.$data.Search.Alias
        }
        let options = {
            title: '请输入标题 ',
            value: ""
        };
        layer.prompt(options, function (value, index, elem) {
            dto.Title = value;
            http.post("CommonExecSql/Add", dto).then(r => {
                top.message.ToastCode(r);
            });
            layer.closeAll();
        });
    }
}



var editor;

// 最小高度
var MIN_HEIGHT = 100;

//对编辑器这个node添加鼠标事件
var editorNode = document.getElementById('code');

var hahahha;
//var tablewords = {};
var dragBar = document.getElementById('handle');
var SqlEditor = {
    //根据DOM元素的id构造出一个编辑器
    InitEditor: () => {
        editor = CodeMirror.fromTextArea(document.getElementById("SearchSql"), {
            mode: "text/c-mysql", //实现Java代码高亮
            lineNumbers: true,
            theme: "default",
            keyMap: "default",
            extraKeys: { "Tab": "autocomplete" },
            hint: CodeMirror.hint.sql,
            lineWrapping: true,         //是否换行
            foldGutter: true,           //是否折叠
            gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter"], //添加行号栏，折叠栏
            hintOptions: {
                tables: window.tablewords
            }
        });
        hahahha = document.getElementsByClassName('CodeMirror-wrap')[0];
    },
    //注册事件
    RegientEvent: () => {
        editor.on("keyup", function (cm, event) {
            //所有的字母和'$','{','.'在键按下之后都将触发自动完成
            if (!cm.state.completionActive &&
                ((event.keyCode >= 65 && event.keyCode <= 90) || event.keyCode == 52 || event.keyCode == 219 || event.keyCode == 190)) {
                CodeMirror.commands.autocomplete(cm, null, { completeSingle: false });
            }
        });

        dragBar.addEventListener('mousedown', function (e) {
            console.log('start');
            // 开始高度
            startHeight = SqlEditor.getHeight(hahahha);
            console.log('开始高度：' + startHeight);
            pos_x = e.x;
            pos_y = e.y;



            //只有按下鼠标移动的时候才能移动
            document.body.addEventListener('mousemove', SqlEditor.drag);
            window.addEventListener('mouseup', SqlEditor.release);
        });

    },
    // 格式化
    FormatSql: () => {
        var range = SqlEditor.getSelectedRange();
        editor.autoFormatRange(range.from, range.to);
        SqlEditor.format();
    },
    format: function () {
        console.time('formatting');

        var str = sqlFormatter.format(editor.getValue(), { language: 'sql' });
        editor.setValue(str);
        console.log('格式化的代码:' + str);
        console.timeEnd('formatting');
    },

    /**
 * 根据sql编辑框动态高度，设置表格的高度
 */
    setDatalistHeight: () => {
        // $("#handle").show(); 
        var allH = parseInt($(window).height());
        var sqlDivHeight = parseInt($('.editzone').height());
        var toolsDivHeight = parseInt($('.btnzone').height());
        var datalistH = allH - sqlDivHeight - toolsDivHeight - 32 - 30;
        $('.result').height(datalistH + 'px');
        $('.main').height(datalistH + 'px');
        document.querySelectorAll(".result .layui-tab-item").forEach(x => {
            x.style.height = (datalistH - 50) + "px";

        });


    },
    //// 返回编辑器的高度
    getHeight: (node) => {
        var h = window.getComputedStyle(node, null).height.replace(/px$/, "");
        if (h === 'auto') {
            h = node.offsetHeight;
        }
        return parseInt(h);
    },

    // 释放鼠标的时候触发的事件
    release: () => { 
        // 删除和添加都是使用两个参数的
        document.body.removeEventListener('mousemove', SqlEditor.drag);
        window.removeEventListener('mouseup', SqlEditor.release);

    },
    // drag 事件
    drag: function (e) {
        editor.setSize('height', Math.max(MIN_HEIGHT, (startHeight + e.y - pos_y)) + "px");
        SqlEditor.setDatalistHeight();// 数据表格高度
    },

    getSelectedRange: function () {
        return { from: editor.getCursor(true), to: editor.getCursor(false) };
    }




}







var tmp = function () {
    SqlEditor.InitEditor();
    SqlEditor.RegientEvent();
    editor.refresh();
};



$(() => {
    P.Init();


    setTimeout(tmp, 1000);
    // 监听localStorage的变化事件
    window.addEventListener("storage", function (event) {
        // 获取变化的键值和新值
        var key = event.key;
        var newValue = event.newValue;
        // 判断是否是我们需要监听的键值
        if (key === "searchsql") {
            // 更新计数器的值
            P.SetSql(newValue);
        }
    });
})