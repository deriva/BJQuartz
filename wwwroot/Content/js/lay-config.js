/**
 * date:2019/08/16
 * author:Mr.Chung
 * description:此处放layui自定义扩展
 * version:2.0.4
 */

window.rootPath = (function(src) {
	src = document.scripts[document.scripts.length - 1].src;
	return src.substring(0, src.lastIndexOf("/") + 1);
})();

layui.config({
	base: rootPath ,
	version: true
}).extend({
	miniAdmin: "lay-module/layuimini/miniAdmin", // layuimini后台扩展
	miniMenu: "lay-module/layuimini/miniMenu", // layuimini菜单扩展
	miniTab: "lay-module/layuimini/miniTab", // layuimini tab扩展
	miniTheme: "lay-module/layuimini/miniTheme", // layuimini 主题扩展
	miniTongji: "lay-module/layuimini/miniTongji", // layuimini 统计扩展
	step: 'lay-module/step-lay/step', // 分步表单扩展
	treetable: 'lay-module/treetable-lay/treetable', //table树形扩展
	tableSelect: 'lay-module/tableSelect/tableSelect', // table选择扩展
	iconPickerFa: 'lay-module/iconPicker/iconPickerFa', // fa图标选择扩展
	echarts: 'lay-module/echarts/echarts', // echarts图表扩展
	echartsTheme: 'lay-module/echarts/echartsTheme', // echarts图表主题扩展
	wangEditor: 'lay-module/wangEditor/wangEditor', // wangEditor富文本扩展
	layarea: 'lay-module/layarea/layarea', //  省市县区三级联动下拉选择器,
	power:'power',//权限
 
 
});
