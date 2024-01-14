var Search = {
 	GetGuid() {
 		function S4() {
 			return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
 		}
 		return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
 	},
 	SerchDialog: function(e) {
 		$(".dialogSearchList").empty();
 		var pre = $(e).attr("data-pretime");
 		if (Date.parse(new Date()) - pre > 1000 || pre == 0) {
 			var dom = $(e).parent().find(".dialogSearchList"); //定位
 			var wt = $(dom).css("width");

 			var id = "";
 			var iframe = $(dom).find("iframe");
 			if (iframe.length == 0) {
 				id = "ifmDialog" + Date.parse(new Date());
 				var spanid = "spcancel" + id;
 				var cancelhtml =
 					'<svg t="1616760280415" class="icon" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="3130" width="32" height="32"><path d="M513.43007 1019.262092c-280.20375 0-507.388982-227.207745-507.388982-507.410472 0-280.224216 227.185232-507.409448 507.388982-507.409448 280.247752 0 507.391029 227.185232 507.391029 507.409448C1020.821099 792.054347 793.678846 1019.262092 513.43007 1019.262092zM746.107387 363.903034c9.540284-9.53926 9.540284-25.021883 0-34.539654l-51.822272-51.800783c-9.535167-9.558703-24.977881-9.558703-34.518165 0L512.976746 424.334381 366.184495 277.562597c-9.53619-9.558703-24.977881-9.558703-34.518165 0l-51.822272 51.800783c-9.538237 9.517771-9.538237 25.001417 0 34.539654l146.793274 146.770761-146.793274 146.790204c-9.538237 9.518794-9.538237 25.004487 0 34.540677l51.822272 51.79976c9.540284 9.538237 24.981974 9.538237 34.518165 0L512.976746 597.014232l146.790204 146.790204c9.540284 9.538237 24.982998 9.538237 34.518165 0l51.822272-51.79976c9.540284-9.53619 9.540284-25.021883 0-34.540677L599.317183 510.674818 746.107387 363.903034z" p-id="3131"></path></svg>';
 				var html = "<span onclick='Search.HideDialog(\"" + spanid + "\")' id='" + spanid +
 					"' style='position: absolute;right: -11px;top: -17px; cursor: pointer;display: block;color: darkmagenta;'>" +
 					cancelhtml + "</span>";

 				html += "<iframe id=" + id + " src=" + url + " width='" + wt + "' " +
 					" style='' tabindex='12'/>";
 				$(dom).html(html);
 			} else {
 				id = $(iframe).attr("id");
 			}
 			var cb = $(e).attr("data-cb");
 			$(dom).attr("id", "dialogSearch" + id);
 			var url = $(e).attr("data-url");
 			if (url.indexOf("?") == -1) url += "?";
 			url += "&cb=" + cb;
 			var searchkey = $(e).attr("data-searchkey");
 			if (searchkey.length == 0) searchkey = "SearchKey";
 			url += "&" + searchkey + "=" + e.value

 			var eleid = $(e).attr("id");
 			if (!eleid) {
 				eleid = Search.GetGuid();
 				$(e).attr("id", eleid);
 			}
 			url += "&domid=" + id;
			$(dom).attr("id", "dialogSearch" + id);
 			$(dom).find("iframe").attr("src", url);

 			event.stopPropagation();
 			event.preventDefault();
 			// console.log(Date.parse(new Date()))
 			$(e).attr("data-pretime", Date.parse(new Date()));
 			$(e).parent().find(".dialogSearchList").show();
 			Search.ResizeHegiht(id);
 		}
 	},

 	ResizeHegiht: function(id) { //重新释放高度
 		setTimeout(function() {
 			var iframe = document.getElementById(id);
 			try {
 				var bHeight = iframe.contentWindow.document.body.scrollHeight;
 				var dHeight = iframe.contentWindow.document.documentElement.scrollHeight;
 				var height = Math.min(bHeight, dHeight);
 				iframe.height = height+100;

 			} catch (ex) {}

 		}, 300);


 	},
 	HideDialog: function(domid) {
 		if ($("#" + domid).parent().hasClass("dialogSearchList")) $("#" + domid).parent().hide();
 		else
 			$("#" + domid).parent().find(".dialogSearchList").hide(); //将弹窗隐藏掉

 	},
 	TabEvent: function(e) {
 		//  var code = event.ke
 		var keycode = window.event.keyCode;

 		if (event.pointerType == "mouse") {
 			$("[data-row]").removeClass("buddyListHighLight");
 			$(e).addClass("buddyListHighLight");
 			// $(e).find("[value='选择']").click();
 		} else {

 			var currrow = $(e).attr("data-row");
 			switch (keycode) {
 				case 38: //向上
 					currrow--;
 					if (currrow < 0) currrow = 0;
 					$("[data-row]").removeClass("buddyListHighLight");
 					$("[data-row=" + currrow + "]").addClass("buddyListHighLight");
 					var height = $("[data-row=" + currrow + "]").height();
 					$("[data-row=" + currrow + "]").scrollTop(height);
 					break;
 				case 40: //向下
 					currrow++;
 					if ($("[data-row=" + currrow + "]").length == 0) {
 						currrow--;
 					} //如果不存在就恢复原来选中的行
 					$("[data-row]").removeClass("buddyListHighLight");
 					$("[data-row=" + currrow + "]").addClass("buddyListHighLight");
 					var height = $("[data-row=" + currrow + "]").height();
 					$("[data-row=" + currrow + "]").scrollTop(height);
 					break;
 				case 13: //回车
 					$(e).find("[value='选择']").click();
 					break;
 				default:
 					break;
 			}
 		}
 	}

 }



 document.onkeydown = function(e) {
 	e = window.event || e;
 	var dom = $(".buddyListHighLight");
 	if (dom == null) dom = $("[data-row=0]");
 	if (dom.length > 0)
 		Search.TabEvent(dom);
 }
