var CrudTmpl = {
	ReaderOps: function(ops) {
		var table;
		var form;
		layui.use(['form', 'table', 'tableSelect', 'power'], function() {
			form = layui.form,
				table = layui.table,
				power = layui.power;
			if (CrudTmpl.IsEmpty(ops.toolbar)) {
				ops.toolbar = '#toolbarDemo'
			}
			if (CrudTmpl.IsEmpty(ops.id)) {
				ops.id = 'tablist'
			}
			if (CrudTmpl.IsEmpty(ops.elem)) {
				ops.elem = '#currentTableId'
			}

			//if (CrudTmpl.IsEmpty(ops.delcb)) { ops.delcb = 'P.DelCb' }//删除页回调函数
			if (CrudTmpl.IsEmpty(ops.listpagecb)) {
				ops.listpagecb = 'P.ListPageCb'
			} //编辑页回调函数
			if (CrudTmpl.IsEmpty(ops.editpagecb)) {
				ops.editpagecb = 'P.EditPageCb'
			} //编辑页回调函数
			if (CrudTmpl.IsEmpty(ops.viewpagecb)) {
				ops.viewpagecb = 'P.ViewPageCb'
			} //详情页回调函数
			if (CrudTmpl.IsEmpty(ops.editpageotherbtncb)) {
				ops.editpageotherbtncb = 'P.EditPageOtherBtncb'
			} //详情页回调函数

			if (CrudTmpl.IsEmpty(ops.editpagewidth)) {
				ops.editpagewidth = 0
			} //编辑页的宽
			if (CrudTmpl.IsEmpty(ops.editpageheight)) {
				ops.editpageheight = 0
			} //编辑页的高度
			if (CrudTmpl.IsEmpty(ops.viewpagewidth)) {
				ops.viewpagewidth = 0
			} //详情页页的宽
			if (CrudTmpl.IsEmpty(ops.viewpageheight)) {
				ops.viewpageheight = 0
			} //详情页的高度
			if (CrudTmpl.IsEmpty(ops.pagesize)) {
				ops.pagesize = 20
			} //详情页的高度
			if (CrudTmpl.IsEmpty(ops.where)) {
				ops.where = $("#fm").serializeJson();
			} //详情页的高度
			ops.where.token = http.getToken();
			table.render({
				elem: ops.elem,
				url: ops.url, 
				where: ops.where,
				toolbar: ops.toolbar,
				id: ops.id,
				defaultToolbar: ['filter', 'exports', 'print', {
					title: '提示',
					layEvent: 'LAYTABLE_TIPS',
					icon: 'layui-icon-tips'
				}],
				cols: ops.cols,
				parseData: function(res) { //res 即为原始返回的数据
					if (res.code != 100) {
						LP.ToastCode(res);
					} else {
						if (res.hasOwnProperty("Page")) {
							return {
								"code": 0, //解析接口状态
								"msg": res.message, //解析提示文本
								"count": res.TotalCount, //解析数据长度
								"data": res.Items //解析数据列表
							};
						} else {

							return {
								"code": 0, //解析接口状态
								"msg": res.message, //解析提示文本
								"count": res.attr.TotalCount, //解析数据长度
								"data": res.attr.DataSource //解析数据列表
							};
						}
					}
				},
				done: function(res, curr, count) {
					// Tmpl.Init();
					if (!CrudTmpl.IsEmpty(ops.listpagecb)) {
						try {
							eval(ops.listpagecb + "('" + JSON.stringify(res) + "','" +
								curr + "','" + count + "')");
						} catch {}
					}
					power.CheckFunctionKeys();//检查权限key是否合法
				},
				limits: [10, 15, 20, 25, 50, 100],
				limit: ops.pagesize,
				page: true,
				skin: 'line'
			});

			// 监听搜索操作
			form.on('submit(data-search-btn)', function(data) {
				var result = JSON.stringify(data.field);
				table.reload(ops.id, {
					where: ops.where,
					page: {
						curr: 1
					}
				}, false);

				return false;
			});

			/**
			 * toolbar监听事件
			 */
			table.on('toolbar(currentTableFilter)', function(obj) {
				if (obj.event === 'add') { // 监听添加操作
					var html = $("#curredit").html();
					LP.DialogCRDReaderDataTmpl(null, html, ops.editpagewidth, ops
						.editpageheight);

					Tmpl.Start("#fmEdit");
					LP.Init();
					CrudTmpl.InitTabSelect("#fmEdit");
					layui.use('form', function() {
						var form = layui.form;
					});
					if (!CrudTmpl.IsEmpty(ops.editpagecb)) {
						try {
							eval(ops.editpagecb + "('')");
						} catch {}
					}
				} else { //列表页其他按钮事件
					if (!CrudTmpl.IsEmpty(ops.listpageotherbtncb)) { //列表页的其他按钮糊掉事件
						var str = "";
						try {
							eval(ops.listpageotherbtncb + "('" + str + "','" + obj.event +
								"','0')");
						} catch {}
					}
				}
			});



			//监听表格复选框选择
			table.on('checkbox(currentTableFilter)', function(obj) {
				// console.log(obj)
			});
			//编辑行里的增删改查
			table.on('tool(currentTableFilter)', function(obj) {
				var data = obj.data;
				// console.log(data);
				if (obj.event === 'view') { //查询
					var html = $("#currdetail").html();
					LP.DialogCRDReaderDataTmpl(data, html, ops.viewpagewidth, ops
						.viewpageheight, data.Title);
					Tmpl.Start("#divcurrdetail");
					//   Tmpl.IdToName();//渲染后台人员
					LP.GlobalOperationButtonRegister();
					if (!CrudTmpl.IsEmpty(ops.viewpagecb)) {

						try {
							eval(ops.viewpagecb + "('" + JSON.stringify(data) + "','" + data
								.Id + "')");
						} catch {
							return;
						}
					}

					// return  ;
				} else if (obj.event === 'edit') {
					var html = $("#curredit").html();
					LP.DialogCRDReaderDataTmpl(data, html, ops.editpagewidth, ops
						.editpageheight, data.Title);
					CrudTmpl.ReaderConfig();
					if (!CrudTmpl.IsEmpty(ops.editpagecb)) {
						var str = "";
						if (data) {
							str = JSON.stringify(data);
						}
						eval(ops.editpagecb + "('" + str + "','" + data.Id + "')");
					}

				} else if (obj.event === 'delete') { // 监听删除操作
					var checkStatus = table.checkStatus('currentTableId');
					LP.CommonAjax(ops.deleteurl, {
						id: data.Id
					}, "确认取消吗?", function(res) {
						if (!CrudTmpl.IsEmpty(ops.delcb)) {
							try {
								eval(ops.delcb + "('" + JSON.stringify(data) + "','" +
									data.Id + "')");
							} catch {
								// window.location.reload();
							}
						} else {
							window.location.reload();
						}
					});

				} else {
					if (!CrudTmpl.IsEmpty(ops.editpageotherbtncb)) {
						try {
							eval(ops.editpageotherbtncb + "('" + JSON.stringify(data) + "','" +
								obj.event + "','" + data.Id + "')");
						} catch {}
					}
				}
			});
		});
	},
	ReaderConfig: function() {
		Tmpl.Start("#fmEdit");
		LP.Init();
		CrudTmpl.InitTabSelect("#fmEdit");
	},

	///渲染下拉选择框
	ReaderSelectOps: function(ops) {
		if (CrudTmpl.IsEmpty(ops.searchKey)) {
			ops.searchKey = 'keyword'
		}
		if (CrudTmpl.IsEmpty(ops.checkedKey)) {
			ops.checkedKey = 'Id'
		}
		if (CrudTmpl.IsEmpty(ops.elem)) {
			ops.elem = '#searchkey'
		}
		if (CrudTmpl.IsEmpty(ops.inputcolumn)) {
			ops.inputcolumn = 'Desc'
		} //删除页回调函数
		if (CrudTmpl.IsEmpty(ops.chainto)) {
			ops.chainto = 'Id'
		} //删除页回调函数
		if (CrudTmpl.IsEmpty(ops.searchPlaceholder)) {
			ops.searchPlaceholder = '关键词搜索'
		} //删除页回调函数

		layui.use(['form', 'table', 'tableSelect'], function() {
			var tableSelect = layui.tableSelect;
			tableSelect.render({
				elem: ops.elem, //定义输入框input对象 必填
				checkedKey: ops.checkedKey, //表格的唯一建值，非常重要，影响到选中状态 必填
				searchKey: ops.searchKey, //搜索输入框的name值 默认keyword
				searchPlaceholder: ops.searchPlaceholder, //搜索输入框的提示文字 默认关键词搜索
				table: { //定义表格参数，与LAYUI的TABLE模块一致，只是无需再定义表格elem
					url: ops.url,
					cols: ops.cols,
					parseData: function(res) { //res 即为原始返回的数据
						return {
							"code": 0, //解析接口状态
							"msg": res.Message, //解析提示文本
							"count": res.Attr.DataSource.length, //解析数据长度
							"data": res.Attr.DataSource //解析数据列表
						};
					},
				},
				done: function(elem, data) {
					var NEWJSON = [];
					var ids = [];
					layui.each(data.data, function(index, item) {

						NEWJSON.push(item[ops.inputcolumn])
						ids.push(item.Id);
					});

					elem.val(NEWJSON.join(","));
					$("#" + ops.chainto).val(ids.join(","));
					if (typeof P.SelectCb == "function") {
						P.SelectCb(elem, data, ops)
					}

				}
			})
		});
	},
	GetTabSelectCols: function(str) {
		var obj = {};
		var typestr = str.split('-')[0] == 0 ? "radio" : "checkbox";
		var type = str.split('-')[1];
		if (type == 0) { //0-0用户下拉联动
			obj.url = "/sysarea/users/QueryListBySearch";
			obj.cols = [
				[{
						type: typestr
					},
					{
						field: 'Id',
						title: 'ID',
						width: 300
					},
					{
						field: 'UserName',
						title: '姓名',
						width: 300
					},
					{
						field: 'DeptName',
						title: '部门',
						width: 300
					},
				]
			];
			obj.inputcolumn = "UserName";
		} else if (type == 1) { //0-1部门下拉联动
			obj.url = "/sysarea/Department/GetDataList";
			obj.cols = [
				[{
						type: typestr
					},
					{
						field: 'Id',
						title: 'ID',
						width: 300
					},
					{
						field: 'Name',
						title: '名称',
						width: 300
					},

				]
			];
			obj.inputcolumn = "Name";
		} else if (type == 2) { //0-2车牌下拉联动
			obj.url = "/cararea/CarInfo/QueryListBySearch";
			obj.cols = [
				[{
						type: typestr
					},
					{
						field: 'DriverName',
						title: '司机姓名',
						width: 100
					},
					{
						field: 'CarNo',
						title: '车牌',
						width: 300
					},
				]
			];
			obj.inputcolumn = "CarNo";
		} else if (type == 3) { //0-3获取用户账号
			obj.url = "/sysarea/users/QueryListBySearch";
			obj.cols = [
				[{
						type: typestr
					},
					{
						field: 'Id',
						title: 'ID',
						width: 300
					},
					{
						field: 'Account',
						title: '账号',
						width: 300
					},
					{
						field: 'UserName',
						title: '姓名',
						width: 300
					},
				]
			];
			obj.inputcolumn = "Account";
		} else if (type == 4) { //0-4获取会议室
			obj.url = "/WorkArea/MeetingConfig/QueryListBySearch";
			obj.cols = [
				[{
						type: typestr
					},
					{
						field: 'Id',
						title: 'ID',
						width: 100
					},
					{
						field: 'Desc',
						title: '会议室',
						width: 300
					},
				]
			];
			obj.inputcolumn = "Desc";

		} else if (type == 5) { //0-5获取物品名称与ID
			obj.url = "/ProductArea/ProductInfo/GetDataList";
			obj.cols = [
				[{
						type: typestr
					},
					{
						field: 'Id',
						title: '物品编号',
						width: 300
					},
					{
						field: 'Name',
						title: '物品名称',
						width: 300
					},
				]
			];
			obj.inputcolumn = "Name";
		} else if (type == 6) { //0-6获取用户名与工号
			obj.url = "/sysarea/users/GetDataList?Status=1";
			obj.cols = [
				[{
						type: typestr
					},
					{
						field: 'No',
						title: '工号',
						width: 300
					},
					{
						field: 'UserName',
						title: '姓名',
						width: 300
					},
				]
			];
			obj.inputcolumn = "UserName";
		} else if (type == 7) { //0-7获取IT部门用户名
			obj.url = "/sysarea/users/GetITDataList?";
			obj.cols = [
				[{
						type: typestr
					},
					{
						field: 'Id',
						title: 'Id',
						width: 300
					},
					{
						field: 'UserName',
						title: '姓名',
						width: 300
					},
				]
			];
			obj.inputcolumn = "UserName";
		} else if (type == 8) { //0-8获取角色用户名
			obj.url = "/sysarea/RoleV2/GetDataList?IsEnable=true";
			obj.cols = [
				[{
						type: typestr
					},
					{
						field: 'Id',
						title: 'Id',
						width: 300
					},
					{
						field: 'RoleName',
						title: '角色',
						width: 300
					},
				]
			];
			obj.inputcolumn = "RoleName";
		}
		return obj;

	},
	InitTabSelect: function(pre) {
		if (pre == undefined) pre = "";
		$(pre + " [bctabselect]").each(function(i, dom) {
			var id = $(dom).attr("Id");
			var str = $(dom).attr("bctabselect"); //0-0  代表单选框-用户联动搜索
			if (CrudTmpl.IsEmpty(str)) return false;

			if (CrudTmpl.IsEmpty(id)) $(dom).attr("Id", "tabselectusers" + i);
			var chainid = $("#" + id).attr("chainto");
			var obj = CrudTmpl.GetTabSelectCols(str);
			var options = {
				elem: '#' + id, //定义输入框input对象 必填
				url: obj.url,
				chainto: chainid,
				cols: obj.cols,
				inputcolumn: obj.inputcolumn
			}
			CrudTmpl.ReaderSelectOps(options); //渲染增删改查的内容 
		});

	},
	//渲染上传控件
	ReaderUpload: function(ops) {
		if (CrudTmpl.IsEmpty(ops.module)) ops.module = "temp";
		if (CrudTmpl.IsEmpty(ops.previewid)) ops.previewid = "";
		if (CrudTmpl.IsEmpty(ops.filetype)) ops.filetype = "file";
		if (CrudTmpl.IsEmpty(ops.exts)) ops.exts = "doc|pdf|xls|xlsx|zip|rar|7z";
		if (CrudTmpl.IsEmpty(ops.multiple)) ops.multiple = false;
		if (CrudTmpl.IsEmpty(ops.uploadcb)) ops.uploadcb = "P.UploadCb"; //上传回调
		if (CrudTmpl.IsEmpty(ops.type)) ops.type = 3; //3临时文件  2私人文件 1系统文件
		var chainto = $(ops.elem).attr("chainto");
		layupload.render({ //允许上传的文件后缀
			elem: ops.elem,
			url: '/file/Upload?module=' + ops.module + '&type=' + ops.type //改成您自己的上传接口
				,
			accept: 'file' //普通文件
				,
			contenttype: '',
			multiple: ops.multiple //是否允许多文件上传
				,
			type: ops.filetype,
			exts: ops.exts //只允许上传压缩文件
				,
			before: function(obj) { //obj参数包含的信息，跟 choose回调完全一致，可参见上文。
				// layer.load(); //上传loading
				//预读本地文件示例，不支持ie8
				if (ops.filetype == "img" && !CrudTmpl.IsEmpty(ops.previewid)) {
					obj.preview(function(index, file, result) {
						$('#' + ops.previewid).append('<img src="' + result + '" alt="' +
							file.name + '" ondblclick="CrudTmpl.DelUploadFile(this,\'' +
							ops.previewid + '\',\'' + chainto +
							'\')"   class="layui-upload-img"  style="width: 100px;height:100px">'
						)
					});
				}
			},
			done: function(res) {
				layer.closeAll('loading'); //关闭loading
				if (res.Code == 200) {

					if (ops.filetype == "img" && !CrudTmpl.IsEmpty(ops.previewid)) {
						var arr = [];
						var initval = $("input[chainto2='" + chainto + "']").val();
						if (initval.length > 0 && !CrudTmpl.IsEmpty(initval))
							arr = initval.split(',');
						arr.push(res.Attr);
						$('#' + ops.previewid).empty();
						arr.forEach(function(value, index) {
							$('#' + ops.previewid).append('<img src=' + value +
								'  class="layui-upload-img"  onclick="CrudTmpl.DelUploadFile(this,\'' +
								ops.previewid + '\',\'' + chainto +
								'\')" style="width: 100px;height:100px">')
						});
						$("input[chainto2='" + chainto + "']").val(arr.toString());
					} else {

						$("a[chainto2='" + chainto + "']").attr("href", res.Attr);
						$("a[chainto2='" + chainto + "']").show();
						$("input[chainto2='" + chainto + "']").val(res.Attr);
					}
					layer.msg("上传成功", function() {});
					try {
						eval(ops.uploadcb + "('" + ops.elem + "','" + res.Attr + "')");
					} catch {}



				} else {
					layer.msg("上传失败");
				}
			}
		});
	},
	InitUpload: function(pre) {
		if (pre == undefined) pre = "";
		$(pre + " [bcupload]").each(function(i, dom) {
			var id = $(dom).attr("Id");
			var module = $(dom).attr("bcupload");
			var type = $(dom).attr("data-type");
			var exts = $(dom).attr("data-exts");
			var previewid = $(dom).attr("data-previewid");
			var filetype = $(dom).attr("data-filetype");
			var multiple = $(dom).attr("data-multiple") == 1;

			var uploadcb = $(dom).attr("data-uploadcb");
			if (CrudTmpl.IsEmpty(id)) $(dom).attr("Id", "bcupload" + i);

			var options = {
				elem: '#' + id, //定义输入框input对象 必填
				module: module,
				type: type,
				exts: exts,
				filetype: filetype,
				multiple: multiple,
				previewid: previewid,
				uploadcb: uploadcb
			}
			CrudTmpl.ReaderUpload(options); //渲染上传控件
		});
	},
	DelUploadFile: function(e, id, name) {
		var arr = [];
		$(e).remove();
		$("#" + id).find("img").each(function(i, dom) {
			arr.push($(dom).attr("src"));
		});
		if ($("[name='" + name + "']").val() == undefined) {
			$("#" + name + "").val(arr.toString());
		} else {
			$("[name='" + name + "']").val(arr.toString());
		}

	},
	IsEmpty: function(id) {
		if (id == undefined || id == null || id == "" || id == "undefined" || id == "null") return true;
		return false;
	}


}


//exports('CrudTmpl', CrudTmpl);


//模板转换

var Tmpl = {
	JsonDef: function() {
		var enumsconvertkey = "enumsconvertkey";
		var lst = localStorage.getItem(enumsconvertkey);
		if (lst != undefined && lst != "undefined" && lst.length > 0) {
			return JSON.parse(lst);
		}
		$.getJSON("/sysarea/SysEnumsConvert/GetDataList", function(res) {
			localStorage.setItem(enumsconvertkey, JSON.stringify(res.Attr.DataSource));
			return res.Attr.DataSource;
		});
	},
	CarPathJsonData: function() {
		var item = localStorage.getItem("key_carpath_lst");
		if (CrudTmpl.IsEmpty(item)) {
			$.getJSON("/CarArea/CarInfo/GetCarPath?", {}, function(data) {
				var it = JSON.parse(data.Attr);
				var lst2 = [];
				for (var i = 0; i < it.data.length; i++) {
					var it23 = it.data[i];
					lst2.push({
						Id: it23.Id,
						PathName: it23.PathName
					});
				}
				var it2 = JSON.stringify(lst2);
				localStorage.setItem("key_carpath_lst", it2);
				return JSON.parse(it2);
			});
		} else {
			return JSON.parse(item);
		}
	},
	CashTypeJsonData: function() { //初始化银行卡
		var item = localStorage.getItem("key_cashtype_lst");
		if (CrudTmpl.IsEmpty(item)) {
			$.getJSON("/FinalArea/ClaimPayment/GetEnumsCashType", {}, function(data) {
				var it = JSON.parse(data.Attr);
				var lst2 = [];
				for (var i = 0; i < it.data.length; i++) {
					var it23 = it.data[i];
					lst2.push({
						Id: it23.id,
						Name: it23.name
					});
				}
				var it2 = JSON.stringify(lst2);
				localStorage.setItem("key_cashtype_lst", it2);
				return JSON.parse(it2);
			});
		} else {
			return JSON.parse(item);
		}
	},
	ReaderOptions: function(dom, defval, tojsonname, isappend) { //获取下拉框
		var json = Tmpl.JsonDef();
		if (json) {
			var html = "";
			json.forEach(function(di, i) {
				if (di.Modlue == tojsonname && di.ParentId > 0) {
					//如果指定了名称，那就不默认
					if ($(dom).find("option[value=" + di.Val + "]").length == 0) {

						var selected = "";
						if (defval != undefined && defval != null && defval.length > 0 && defval == di
							.Val) {
							selected = "selected='selected'"
						}
						html += "<option value = '" + di.Val + "' " + selected + "  data-desc='" + di
							.Desc + "'>" + di.Desc + "</option >";
					}
				}
			});
			if (isappend == 1) {
				$(dom).append(html);
			} else {
				$(dom).html(html);
			}
		}
	},

	GetText: function(tojsonname, defval) { //获取文本值
		var json = Tmpl.JsonDef();
		if (json) {
			var info = json.filter(item => item.Modlue == tojsonname && item.ParentId > 0 && item.Val ==
				defval);
			if (info.length > 0)
				return info[0].Desc;
		}
		return "";

	},

	StartConvert: function(e) {
		$(e).each(function(i, dd) {
			var tmplstr = $(dd).attr("tmplconvert");
			var fr = tmplstr.split(',');
			var name = title = tojsonname = type = defval = "";
			var isemptytitle = 1;
			var isappend = 0;
			var tf = 0; //时间格式
			type = $(dd).prop("tagName"); ////类型
			if (fr[0].indexOf("tf:") > -1) { //如果是时间格式转化  tf:1
				var val = fr[1].replace("d:", "");
				if (val != undefined && val != null && val != "undefined" && val != "null" && val !=
					"") {
					tf = fr[0].replace("tf:", "");
					$(dd).attr("title", val);
					if (tf == 1) val = Tmpl.FormatDate(val, "yyyy-MM-dd");
					if (tf == 2) val = Tmpl.FormatDate(val, "yyyy-MM-dd hh:mm:ss");
					if (tf == 3) val = Tmpl.FormatDate(val, "MM-dd hh:mm");
					if (tf == 4) val = Tmpl.FormatDate(val, "yyyy-MM");
					$(dd).html(val);
				} else {
					$(dd).html("");
				}
			} else {
				fr.forEach(function(dom, i) {
					if (dom.indexOf("d:") > -1) {
						defval = dom.split(':')[1];
					} //是否默认值
					if (dom.indexOf("tj:") > -1) {
						tojsonname = dom.split(':')[1];
					} //tojson 转换成json值
					if (dom.indexOf("ap:") > -1) {
						isappend = dom.split(':')[1];
					} //是否是追加

				});


				if (type.toLowerCase() == "select") { //下拉框
					var html = Tmpl.ReaderOptions(dd, defval, tojsonname, isappend);
					$(dd).html(html);
				} else { //文本替换值
					var html = Tmpl.GetText(tojsonname, defval);
					$(dd).html(html);
				}
			}
		});

	},
	IdToName: function(pre) { //Id转Name
		if (pre == undefined) pre = "";
		var domele = pre + " [bcidtoname]";

		var len = $(domele).length;
		if (len > 0) {
			var arr = [];
			$(domele).each(function(i, dom) {
				var str = $(dom).attr("bcidtoname").split('-');
				var tina = arr.filter((p) => {
					return p.type == str[0];
				});
				if (tina && tina.length > 0) {
					if (tina[0].ids.indexOf(str[1]) == -1)
						tina[0].ids.push(str[1]);
				} else {
					var sa = [];
					sa.push(str[1]);
					arr.push({
						type: str[0],
						ids: sa
					});
				}


			});
			for (var i = 0; i < arr.length; i++) {
				Tmpl.IdToNameSub(pre, arr[i]);
			}

		}
	},
	IdToNameSub: function(pre, info) {
		var url = "";
		var type = info.type;
		var name = "";
		if (type == 0) {
			url = "/sysarea/Users/QueryListName?Status=1";
			name = "UserName";
		} //用户
		if (type == 1) {
			url = "/sysarea/Users/QueryRoleNameByUserId?Status=1";
			name = "RoleName";
		} //用户转角色
		if (type == 2) {
			url = "/sysarea/Department/QueryNameByUserId?Status=1";
			name = "Name";
		} //部门
		if (type == 3) {
			url = "/ProductArea/ProductInfo/QueryNameByProductId?Status=1";
			name = "Name";
		} //物品编号转名称
		if (type == 4) {
			url = "/sysarea/Users/QueryUsersNameByNo?Status=1";
			name = "UserName";
		} //工号转名称

		if (type == 6) {
			name = "PathName";
			return Tmpl.IdToPathName(pre, info.ids.toString(), name, type);
		} //取专线
		if (type == 7) {
			name = "Name";
			return Tmpl.IdToCashTypeName(pre, info.ids.toString(), name, type);
		} //取银行卡号
		if (type == 8) {
			url = "/sysarea/RoleV2/GetDataList?PageSize=100&IsEnable=true";
			name = "RoleName";
		} //角色Id转名称
		if (info.ids.toString().length == 0) return;
		$.getJSON(url, {
			ids: info.ids.toString()
		}, function(res) {
			if (res.Attr != null) {
				var lst = res.Attr;
				if (res.Attr["DataSource"] && res.Attr["DataSource"].length > 0)
					lst = res.Attr.DataSource;

				for (var t = 0; t < lst.length; t++) {
					var re = lst[t];
					var nsm = re[name];
					//if (nsm != undefined)
					//    if (t < res.Attr.length - 1) { nsm += " "; }
					var chaitoobj = $(pre + " [bcidtoname=" + type + "-" + re.Id + "]").parent().find(
						"[chainto]");
					if ($(chaitoobj).length > 0) {
						$(chaitoobj).val(nsm);

					} else {
						$(pre + " [bcidtoname=" + type + "-" + re.Id + "]").text(nsm);
					}
				}

			}
		});
	},
	IdToCashTypeName: function(pre, ids, name, type) { //ID转银行卡名称
		var lst = Tmpl.CashTypeJsonData();
		if (lst != null && lst.length > 0) {
			for (var t = 0; t < lst.length; t++) {
				var re = lst[t];
				var nsm = re[name];
				if (nsm != undefined)
					if (t < lst.length - 1) {
						nsm += "";
					}
				if (ids.length > 0) {
					var idlst = ids.split(',');
					for (var i = 0; i < idlst.length; i++) {
						if (idlst[i] == re.Id) {
							$(pre + " [bcidtoname=" + type + "-" + idlst[i] + "]").append(nsm);

						}
					}
				}
			}
		}
	},
	IdToPathName: function(pre, ids, name, type) { //ID转专线名称
		var lst = Tmpl.CarPathJsonData();
		if (lst != null && lst.length > 0) {
			for (var t = 0; t < lst.length; t++) {
				var re = lst[t];
				var nsm = re[name];
				if (nsm != undefined)
					if (t < lst.length - 1) {
						nsm += ",";
					}
				if (ids.length > 0) {
					var idlst = ids.split(',');
					for (var i = 0; i < idlst.length; i++) {
						if (idlst[i] == re.Id)
							$(pre + " [bcidtoname=" + type + "-" + idlst[i] + "]").append(nsm);
					}
				}
			}
		}
	},
	GetTimeHtml: function(val, tfval) { //获取时间html
		return '<span tmplconvert="tf:' + tfval + ',d:' + val + '" > ' + val + '</span >';
	},
	FormatDate: function(tt, fmt) {
		if (tt) {
			tt = tt.replace(new RegExp("-", "g"), "/"); //兼容苹果浏览器
			var dd = new Date(tt);
			var o = {
				"M+": dd.getMonth() + 1, //月份
				"d+": dd.getDate(), //日
				"h+": dd.getHours(), //小时
				"m+": dd.getMinutes(), //分
				"s+": dd.getSeconds(), //秒
				"q+": Math.floor((dd.getMonth() + 3) / 3), //季度
				"S": dd.getMilliseconds() //毫秒
			};
			if (/(y+)/.test(fmt)) {
				fmt = fmt.replace(RegExp.$1, (dd.getFullYear() + "").substr(4 - RegExp.$1.length));
			}
			for (var k in o) {
				if (new RegExp("(" + k + ")").test(fmt)) {
					fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" +
						o[k]).length)));
				}
			}
			return fmt;
		}
		return "";
	},
	Start: function(pre) { //内部页面初始化：编辑，详情页称为内页
		//将页面上的状态，枚举值进行标签转换（不需要取数据库里的）
		$(pre + " [tmplconvert]").each(function(i, dom) {
			Tmpl.StartConvert(dom);
		});
		//将页面上的UserId转化为人名（需要取数据库）
		Tmpl.IdToName(pre);
	},


	///页面初始化
	Init: function() {
		Tmpl.Start("");


	}

}



/* $(function() {
	Tmpl.Init(); //初始化
}); */
//exports('Tmpl', Tmpl);
