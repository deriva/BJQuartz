//import { search } from "../jqueryplus/layui/formSelects-v4";

var TabSelct = {
    IsEmpty: function (id) {
        if (id == undefined || id == null || id == "") return true;
        return false;
    },
    ///渲染下拉选择框
    ReaderSelectOps: function (ops) {
        if (TabSelct.IsEmpty(ops.searchKey)) { ops.searchKey = 'searchkey' }
        if (TabSelct.IsEmpty(ops.checkedKey)) { ops.checkedKey = 'Id' }
        if (TabSelct.IsEmpty(ops.elem)) { ops.elem = '#searchkey' }
        if (TabSelct.IsEmpty(ops.cb)) { ops.cb = '' }
        if (TabSelct.IsEmpty(ops.inputcolumn)) { ops.inputcolumn = 'Desc' }//删除页回调函数
        if (TabSelct.IsEmpty(ops.chainto)) { ops.chainto = 'ID' }//删除页回调函数
        if (TabSelct.IsEmpty(ops.searchPlaceholder)) { ops.searchPlaceholder = '关键词搜索' }//删除页回调函数

        layui.use(['form', 'table', 'tableSelect'], function () {
            var tableSelect = layui.tableSelect;
            tableSelect.render({
                elem: ops.elem,	//定义输入框input对象 必填
                checkedKey: ops.checkedKey, //表格的唯一建值，非常重要，影响到选中状态 必填
                searchKey: ops.searchKey,	//搜索输入框的name值 默认keyword
                searchPlaceholder: ops.searchPlaceholder,	//搜索输入框的提示文字 默认关键词搜索 
                size: 'sm', //小尺寸的表格
                table: {	//定义表格参数，与LAYUI的TABLE模块一致，只是无需再定义表格elem
                    url: http.baseURL() + ops.url,
                    cols: ops.cols,
                    headers: { token: http.getToken() },
                    request: {
                        limitName: 'pagesize' //每页数据量的参数名，默认：limit
                    },
                    parseData: function (r) { //res 即为原始返回的数据 
                        var lst = [];
                        if (r.attr && "DataSource" in r.attr) {
                            lst = r.attr.DataSource;
                            totalCount = r.attr.TotalCount;
                        } else {
                            lst = r.Items;
                            totalCount = r.TotalCount;
                        }
                        return {
                            "code": r.code == 100 ? 0 : r.code, //解析接口状态
                            "msg": r.message, //解析提示文本
                            "count": totalCount, //解析数据长度
                            "data": lst //解析数据列表
                        };
                    },
                },
                done: function (elem, data) {
                    var NEWJSON = []; var ids = [];
                    layui.each(data.data, function (index, item) {
                        NEWJSON.push(item[ops.inputcolumn])
                        ids.push(item[ops.primarykey]);
                    });
                    elem.val(NEWJSON.join(","));
                    $("#" + ops.chainto).val(ids.join(","));
                    $("#" + ops.chainto).change();
                    $(elem).change();
                    try {
                        if (!TabSelct.IsEmpty(ops.cb)) {
                            try { eval(ops.cb + "('" + elem.selector + "','" + JSON.stringify(data) + "')"); } catch { }

                        }
                    } catch (e) { }

                }
            })
        });
    },
    GetTabSelectCols: function (str) {//获取下拉联动搜索
        var obj = { primarykey: "ID" };//默认主键：ID
        var typestr = str.split('-')[0] == 0 ? "radio" : "checkbox";
        var type = str.split('-')[1];
        if (type == 1) {//用户
            obj.url = "/userapi/sys_user_ex/GetPageList?";
            obj.cols = [[
                { type: typestr },
                { field: 'account', title: '账户', width: 100 },
                { field: 'username', title: '名称', width: 100 }
            ]]; obj.primarykey = "user_id";
            obj.inputcolumn = "username";
        }
        else if (type == 2) {//客户
            obj.url = "/mbapi/MbInfo/GetSelectSearch?";
            obj.cols = [[
                { type: typestr },
                { field: 'UserNo', title: '客编', width: 100 },
                { field: 'NickName', title: '昵称', width: 100 },
                { field: 'CompanyName', title: '公司名称', width: 130 }
            ]];
            obj.inputcolumn = "NickName"; obj.primarykey = "UserID";
        } else if (type == 3) {//字典
            obj.primarykey = "Key";
            obj.url = "/configapi/Parmater/NoAuthGetDataList?";
            obj.cols = [[
                { type: typestr },
                { field: 'Key', title: 'key', width: 80 },
                { field: 'Name', title: '名称', width: 100 },
                { field: 'Vakue1', title: '值1', width: 80 },
                { field: 'Vakue2', title: '值2', width: 80 }, { field: 'TypeKey', title: '类型key', width: 100 },
                { field: 'TypeName', title: '类型名称', width: 100 }, { field: 'AppName', title: '应用名称', width: 100 },
                { field: 'Remark', title: '备注1', width: 80 }
            ]];
            obj.inputcolumn = "Name";
        } else if (type == 4) {//供应商
            obj.primarykey = "CompanyNo";
            obj.url = "/pcborderapi/company/GetPageList?";
            obj.cols = [[
                { type: typestr }, { field: 'CompanyNo', title: '编号', width: 100 },
                { field: 'CompanyName', title: '工厂名称', width: 300 },

            ]];
            obj.inputcolumn = "CompanyName";
        } else if (type == 5) {//银行卡
            obj.primarykey = "CardKey";
            obj.url = "/configapi/BankCard/GetPageList?";
            obj.cols = [[
                { type: typestr }, { field: 'CardKey', title: '编号', width: 100 },
                { field: 'CardName', title: '名称', width: 200 },
                { field: 'CardNo', title: '卡号', width: 200 },
            ]];
            obj.inputcolumn = "CardName";
        } else if (type == 6) {//字典类型
            obj.primarykey = "TypeKey";
            obj.url = "/configapi/APPParmater/GetDataList?";
            obj.cols = [[
                { type: typestr }, { field: 'TypeKey', title: '类型key', width: 100 },
                { field: 'TypeName', title: '类型名称', width: 200 }, { field: 'AppName', title: '应用名称', width: 100 }
            ]];
            obj.inputcolumn = "TypeName";
        } else if (type == 7) {//文章分类
            obj.primarykey = "ModuleKey";
            obj.url = "/configapi/SystemModuleV2/GetDataList?";
            obj.cols = [[
                { type: typestr }, { field: 'ModuleKey', title: '类型key', width: 150 },
                { field: 'ModuleName', title: '类型名称', width: 200 }
            ]];
            obj.inputcolumn = "ModuleName";
        }
        return obj;

    },
    GetGuid() {
        function S4() {
            return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
        }
        return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
    },
    InitTabSelect: function (pre) {
        if (pre == undefined) pre = "";
        $(pre + " [bctabselect]").each(function (i, dom) {
            var id = $(dom).attr("id");
            if (!id) {
                id = TabSelct.GetGuid();
                $(dom).attr("id", id);
            }
            var parm = $(dom).attr("data-parm");//追加参数
            var cb = $(dom).attr("data-cb");
            if (parm == undefined) parm = "";
            var str = $(dom).attr("bctabselect");//0-1  代表0单选框/1多选-1编号
            if (TabSelct.IsEmpty(str)) return false;

            if (TabSelct.IsEmpty(id)) $(dom).attr("Id", "tabselectusers" + i);
            var chainid = $("#" + id).attr("chainto");
            var obj = TabSelct.GetTabSelectCols(str);
            var options = {
                elem: '#' + id,	//定义输入框input对象 必填
                url: obj.url + parm,
                chainto: chainid,//选中后指向的隐藏域赋值
                cols: obj.cols,//列模板
                inputcolumn: obj.inputcolumn,//选中后输入赋值
                primarykey: obj.primarykey,//选中后的主键key
                cb: cb//选中后回调
            }
            TabSelct.ReaderSelectOps(options);//渲染增删改查的内容 
        });

    },



}


var TabTree = {
    RenderTable: (id,url, cb) => {
        var tree, util;
        layui.use(['tree', 'util'], function () {
            tree = layui.tree, util = layui.util, layer = layui.layer;
            http.get(url).then(r => {
                if (r.code == 100) {
                    var lst = r.attr;
                    lst.forEach((x, i) => { x.i = i });
                    tree.render({
                        elem: id, //可绑定在任意元素中，此处以上述按钮为例
                        data: lst, onlyIconControl: true,
                        click: function (obj) {
                            var info = obj.data;  //获取当前点击的节点数据
                            //ban.$data.Search.CateId = info.id
                            //ban.$data.Search.CateName = info.title;
                            cb(info);
                        }
                    });
                }
            }); 
        });
    },
}
$(function () {
    TabSelct.InitTabSelect("");

});