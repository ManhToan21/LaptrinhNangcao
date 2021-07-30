function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

function API(action) {

    var api = this;
    var http = new XMLHttpRequest();
    http.onload = function () {
        var e = JSON.parse(http.response);
        responsed(e);
    }

    var url = "/api/" + action;
 
    this.post = function (data) {
        http.open('POST', url)
        http.setRequestHeader('Content-type', 'application/json')
        http.send(JSON.stringify(data)) // Make sure to stringify
    }
}

function showMsg(s, e, d) {
    var dlg = new tag().addClass("message-box").html(s);
    if (e) { dlg.data("type", "error"); }

    var mb = dlg.node;
    var p = document.body;
    p.style.position = "relative";
    p.appendChild(mb);

    mb.style.marginLeft = (-mb.offsetWidth / 2) + "px";
    mb.style.marginTop = (-mb.offsetHeight / 2) + "px";

    if (!d) { d = 2000; }
    setTimeout(function () {
        mb.style.opacity = 0;
        setTimeout(function () {
            mb.remove(mb);
        }, 500);
    }, d);
    
}

var ApiMessages = {
    0: "Cập nhật thành công",
};

function responsed(e) {
    var key = Math.abs(e.Code);
    if (e.Code < 0) {
        if (!ApiMessages[key]) {
            ApiMessages[key] = e.Message;
        }
        responseError(e);
    }
    else {
        responseOK(e);
    }
    var s = ApiMessages[key];
    if (s) {
        showMsg(s, e.Code < 0);
    }
}
function responseOK(e) { }
function responseError(e) { }

function beginSearch() {
    //var href = window.location.pathname;
    var v = window.location.pathname;
    var i = v.indexOf('/', 1);
    if (i < 0) {
        v = "/home";
    }
    else {
        v = v.substr(0, i);
    }
    document.getElementById("search-form").action = v + "/search";
}
    
function ApiForm(action, fields, model) {

    VstContainer.apply(this, ["form"]);

    var self = this;

    this.id("main-form").childOf(null);
    var mc = createArt();


    var box = new VstControlBox(fields, "main-form").value(model);
    var footer = box.next(new tag()).addClass("footer row col-lg-12");
    this.submit = footer.child(new tag("a").addClass("btn btn-primary").html("SUBMIT"));

    this.canSubmit = function () {
        return action;
    }
    this.beforeSubmit = function (d) { return d; }

    this.inputs = box.controls;
    this.submit.event("click", function () {
        if (!self.canSubmit()) { return; }

        var data = {
            url: action,
            token: getCookie("token"),
            value: {}
        };
        for (var key in box.controls) {
            var v = box.controls[key].valueOf();
            if (v == null) { return; }

            data.value[key] = v;
        }

        data = self.beforeSubmit(data);
        var api = new API("access/post");
        api.post(data);
    });
}

function ApiTable(url, cols, rows, update, actions) {
    var mc = createArt();

    if (update == null) { update = 7; }

    VstContainer.apply(this);
    this.addClass("progress-table-wrap");
    var table = this.child(new tag()).addClass("progress-table");

    var head = table.child(new tag().addClass("table-head")).id("tab-header");

    var self = this;
    var rowsData;
    var columns;

    function createAct(div, name) {
        div.child(new tag())
            .addClass("btn " + name).html("<i class='fa fa-" + name + "'></i>")
            .event("click", function (n) {
            while (n.tagName != "DIV") { n = n.parentElement; }
            var id = n.id;
            selectedIndex = parseInt(n.parentElement.getAttribute("data-index"));

                self[n.className.split(' ').pop()](selectedIndex);
        });
    }
    this.generate = function (p) {
        columns = {};
        head.html(null);

        for (var i in cols) {
            var col = cols[i];
            var n = col.name;
            var c = col.caption;
            if (!c) { col.caption = c = n; }

            var td = head.child(new tag().addClass(n).html(c));
            columns[n] = col;
        }
        var act = head.child(new tag().addClass("action"));
        if (update & 1) {
            createAct(act, "plus");
        }

        this.childOf(p);
        return this;
    }
    function createRowCells(tr, i) {
        for (var k in columns) {
            var v = rowsData[i][k];
            if (!v) { v = ""; }
            tr.child(new tag()).addClass(k).html(v);
        }
        var act = tr.child(new tag().addClass("action")).data("index", i);

        if (actions) {
            var it = actions.split(',');
            for (var k = 0; k < it.length; k++) {
                var s = it[k].trim();
                createAct(act, s);
            }
        }

        if (update & 2) {
            createAct(act, "edit");
        }
        if (update & 4) {
            createAct(act, "minus");
        }
    }
    function createRow(i) {
        var tr = new tag().addClass("table-row").id("r" + i);
        createRowCells(tr, i);

        return tr;
    }
    this.value = function (data) {
        rowsData = data;
        for (var i in data) {
            table.child(createRow(i));
        }

        return this;
    }
    var editRow = {};
    this.beforeSubmit = function (data) {
        return data;
    }
    this.autoUpdate = function (a, i, v) {
        var url = window.location.pathname.split('/');
        var controllerName = url[1];
        console.log(v);

        var api = new API("access/post/");
        api.post(self.beforeSubmit({
            controller: controllerName,
            action: a,
            objectId: i,
            token: getCookie("token"),
            value: v
        }));
    }
    this.plus = function () {
        new VstModal("Thêm mới", function (b, a) {
            var cb = new VstControlBox(cols);
            b.child(cb);
            a.event("click", function () {
                var value = cb.valueOf();
                if (value == null) {
                    return;
                }
                responseOK = function (e) {
                    rowsData.push(e.Value);
                    head.next(createRow(rowsData.length - 1));
                }
                self.autoUpdate("insert", value.Id, editRow = value);
            });
        }, null, "plus");
    }
    this.edit = function (k) {
        new VstModal("Cập nhật", function (b, a) {
            var cb = new VstControlBox(cols).value(editRow = rowsData[k]);
            b.child(cb);
            a.event("click", function () {
                var value = cb.valueOf();
                if (!value) { showMsg("Error", true); return; }

                for (var i in cols) {
                    var col = cols[i];
                    if (col.tag == "none") continue;

                    editRow[col.name] = value[col.name];
                }

                responseOK = function (e) {
                    var r = document.getElementById("r" + k);
                    rowsData[k] = e.Value;
                    createRowCells(new tag(r).html(null), k);
                }

                self.autoUpdate("update", editRow.Id, editRow);
            });

        }, null, "edit");
    }
    this.minus = function (k) {
        new VstModal("Xóa", function (b, a) {
            var dl = new tag("dl");
            editRow = rowsData[k];
            for (var key in columns) {
                dl.child(new tag("dt").html(columns[key].caption));
                dl.child(new tag("dd").html(editRow[key]));
            }
            b.child(dl);
            a.addClass("btn-danger");
            a.event("click", function () {

                responseOK = function () {
                    document.getElementById("r" + k).remove();
                }

                self.autoUpdate("delete", editRow.Id);
            });

        }, null, "del");
    }

    this.rowAt = function (k) {
        return rowsData[k];
    }

    this.generate().value(rows);
}

function createArt(cls) {
    var mc = document.getElementById("main-content");
    if (!mc) { return; }
    mc.innerHTML += "<div class='art-border art-horiz art-top'><div class='art-border'></div></div>"
        + "<div class='art-border art-vert art-left'><div class='art-border'></div></div>"
        + "<div class='art-border art-horiz art-bottom'><div class='art-border'></div></div>"
        + "<div class='art-border art-vert art-right'><div class='art-border'></div></div>";

    if (!mc.className) {
        if (!cls) { cls = "col-lg-6" };
        mc.className = cls;
    }

    return mc;
}