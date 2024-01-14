var JSToXls = {
    base64: (content) => {
        // return window.btoa((encodeURIComponent(content)));
        return window.btoa(unescape(encodeURIComponent(content)));
    },
    format: function (s, c) {
        return s.replace(/{(\w+)}/g,
            function (m, p) {
                return c[p];
            });
    },

    //导出html表格为 excel 文件
    toExcel: function (id, sheetname) {
        var excelContent = $("#" + id).html();
        var excelFile = "<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:x='urn:schemas-microsoft-com:office:excel' xmlns='http://www.w3.org/TR/REC-html40'>";
        excelFile += "<head> <meta charset='utf-8'><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head>";
        excelFile += "<body><table width='50%'  border='1'>";
        excelFile += excelContent;
        excelFile += "</table></body>";
        excelFile += "</html>";
        //定义excel 的sheet名称
        var ctx = { worksheet: sheetname };
        var link = "data:application/vnd.ms-excel;base64," + JSToXls.base64(JSToXls.format(excelFile, ctx));
        var a = document.createElement("a");
        //定义excel 的文件名称 
        a.download = sheetname + ".xls";
        a.href = link;
        a.click();
    },
    //导出html表格(多个表格)为 excel 文件
    toExcels: function (tables, sheetname) {
        var excelContent = "";
        var excelFile = "<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:x='urn:schemas-microsoft-com:office:excel' xmlns='http://www.w3.org/TR/REC-html40'>";
        excelFile += "<head> <meta charset='utf-8'><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head>";
        excelFile += "<body>";
        for (var i = 0; i < tables.length; i++) {
            excelFile += "<table width='50%'  border='1'>";
            excelFile += tables[i];
            excelFile += "</table>";
        }
        excelFile += "</body>";
        excelFile += "</html>";
        //定义excel 的sheet名称
        var ctx = { worksheet: sheetname };
        var link = "data:application/vnd.ms-excel;base64," + JSToXls.base64(JSToXls.format(excelFile, ctx));
        var a = document.createElement("a");
        //定义excel 的文件名称 
        a.download = sheetname + ".xls";
        a.href = link;
        a.click();
    },

    ///导入excel
    ImportXls: (id, cb) => {
        const fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; //文件类型xlsx
        //const input = document.querySelector('input[type="file"]');
        if (document.querySelector("#" + id)!=null)
            document.querySelector("#" + id).remove();

       $("body").append("<input  style='display:none' type='file' id='" + id + "'/>");
        const input = document.querySelector("#" + id);


        const reader = new FileReader();

        input.addEventListener('change', function () {
            const file = input.files[0];
            reader.readAsBinaryString(file);
            reader.onload = function () {
                const data = reader.result;
                const workbook = XLSX.read(data, { type: "binary" });
                var data2 = [];
                for (var i = 0; i < workbook.SheetNames.length; i++) {
                    const sheetName = workbook.SheetNames[i];
                    const sheet = workbook.Sheets[sheetName];
                    const jsonData = XLSX.utils.sheet_to_json(sheet);
                    data2.push({ table: sheetName, lst: jsonData });
                }
                cb(data2);
                input.remove();
                //jsonData.forEach(item => {
                //    //在界面上显示数据
                //    const row = document.createElement('tr');
                //    const cell1 = document.createElement('td');
                //    cell1.textContent = item.name;
                //    row.appendChild(cell1);
                //    const cell2 = document.createElement('td');
                //    cell2.textContent = item.age;
                //    row.appendChild(cell2);
                //    table.appendChild(row);
                //});
            };
        });

        input.click();
    }

}

var word = {
    /**
 * 导出word
 * @param {string | HTMLElement} domOrSelector 选择器或者DOM元素
 * @param {string} fileName 文件名，不含后缀
 * @param {string} style 样式
 */
    export: (domOrSelector, fileName = "word", style = "") => {
        if (!domOrSelector) {
            return Promise.reject("缺少导出元素或者导出元素选择器");
        }

        const dom =
            typeof domOrSelector === "string"
                ? document.querySelector(domOrSelector)
                : domOrSelector;

        if (!dom) {
            return Promise.reject("未获取到元素选择器对应的元素");
        }

        const exportHtml = `
        <html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:w="urn:schemas-microsoft-com:office:word" xmlns="http://www.w3.org/TR/REC-html40">
            <head>
                <meta charset='utf-8'>
                <title>${fileName}</title>
                ${style ? `<style>${style}</style>` : ""}
            </head>
            <body>
                ${dom.outerHTML}
            </body>
        </html>
    `;

        const url =
            "data:application/vnd.ms-word;charset=utf-8," +
            encodeURIComponent(exportHtml);

        var a = document.createElement("a");
        //定义excel 的文件名称 
        a.download = fileName + ".doc";
        a.href = url;
        a.click();
        // download(url, fileName + ".doc");

        return Promise.resolve(true);
    }


}
