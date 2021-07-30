function tag(n) {
    if (n == null) n = "div";
    if (typeof n == "string")
        this.node = document.createElement(n);
    else
        this.node = n;

    this.addClass = function (c) {
        if (!this.node.className) { this.node.className = c; } else { this.node.className += ' ' + c; }
        return this;
    }
    this.html = function (h) {
        this.node.innerHTML = h;
        return this;
    }
    this.text = function (t) {
        this.node.innerText = t;
        return this;
    }
    this.name = function (n) {
        this.node.name = n;
        return this;
    }
    this.id = function (i) {
        this.node.id = i;
        return this;
    }
    this.data = function (n, v) {
        this.node.setAttribute("data-" + n, v);
        return this;
    }
    this.attr = function (n, v) {
        if (!v) {
            this.node.removeAttribute(n);
        } else {
            this.node.setAttribute(n, v);
        }
        return this;
    }
    this.child = function (e) {
        this.node.appendChild(e.node);
        return e;
    }
    this.next = function (e) {
        this.node.parentElement.insertBefore(e.node, this.node.nextElementSibling);
        return e;
    }
    this.value = function (v) { return this; }
    this.event = function (n, f) {
        this.node.addEventListener(n, function (e) {
            f(e.srcElement);
        });
        return this;
    }
    this.show = function (b) {
        this.node.hidden = !b;
        return this;
    }
}

function control(t) {
    tag.apply(this, [t]);

    this.type = function (t) {
        if (t) { this.node.setAttribute("type", t) }
        return this;
    }
    this.caption = function (t) {
        var lab = new tag("label").html(t);
        this.node.parentNode.insertBefore(lab.node, this.node);

        return this;
    }
    this.name = function (n) {
        return this.id(n).attr("name", n);
    }
    this.required = function (b) {
        this.node.required = b;
        return this;
    }
    this.nullable = function (b) {
        return this.required(!b);
    }

    this.addClass("form-control");
}

function input() {
    control.apply(this, ["input"]);
    this.value = function (v) {
        this.attr("value", v);
        return this;
    }
    this.valueOf = function () {
        var v = this.node.value;
        if (!v && this.node.required) { return null; }
        return v == null ? "" : v;
    }

    var self = this;
    this.node.addEventListener("change", function () {
        self.onchanged(self.node.value);
    });
    this.onchanged = function (s) { }
}
function multiline() {
    control.apply(this, ["div"]);
    this.addClass(" multiline");

    this.value = function (v) {
        this.html(v);
        return this;
    }
    this.valueOf = function () {
        var v = this.node.value;
        return this.node.innerHTML;
    }
}
function checkbox() {
    control.apply(this, ["div"]);

    this.html("<span><i class='fa fa-check'></i></span>").addClass("check-box");
    var lab = this.child(new tag("label").html("Caption"));
    var inp = this.child(new input().type("checkbox"));

    this.node.addEventListener("click", function () {
        inp.node.click();
    });
    inp.event("change", function (n) {
        n.parentElement.setAttribute("data-checked", n.checked);
        inp.value(n.checked);
    });

    this.caption = function (t) {
        lab.html(t);
    }
    this.name = function (n) {
        inp.name(n);
    }
    this.value = function (v) {
        var b = v != null && (v == 1 || v == true || v == 'on' || v.indexOf('u') > 0);

        inp.node.checked = b;
        inp.value(inp.node.checked);

        this.data("checked", inp.node.checked);
    }
    this.valueOf = function () { return inp.node.checked; }
}
function select() {
    control.apply(this, ["div"]);
    this.addClass("nice-select");

    var normCls = this.node.className;

    var span = this.child(new tag("span").addClass("current"));
    var ul = this.child(new tag("ul").addClass("list"));

    var inp = this.child(new input());
    inp.node.hidden = true;

    this.onchanged = function (s) { }

    this.value = function (v) {

        inp.value(v);

        var li = ul.node.firstElementChild;
        var found = false;

        while (li) {
            li.className = "option";
            if (!found && li.getAttribute("data-value") == v) {
                li.className += " selected";
                span.html(li.innerText);

                found = true;
            }

            li = li.nextElementSibling;
        }
    }

    var self = this;
    this.event("click", function (n) {
        if (n.tagName == "LI") {
            var li = ul.node.firstElementChild;
            while (li) {
                li.className = "option";
                li = li.nextElementSibling;
            }

            n.className += " selected";

            var v = inp.valueOf();
            if (n.innerText != v) {
                span.html(v = n.innerText);

                inp.value(n.getAttribute("data-value"));

                n = ul.node.parentElement;
                n.className = normCls;

                self.onchanged(v);
            }

            return;
        }

        if (n.className.indexOf("open") >= 0) {
            n.className = normCls;
        } else {
            n.className += " open";
            ul.node.style.width = n.offsetWidth + "px";
        }
    });

    this.options = function (items) {
        if (Array.isArray(items)) {
            for (var i = 0; i < items.length; i++) {
                li = ul.child(new tag("li").data("value", items[i]).html(items[i])).addClass("option");
            }
        } else {
            for (var key in items) {
                li = ul.child(new tag("li").data("value", key).html(items[key])).addClass("option");
            }
        }
    }
    this.name = function (n) { inp.name(n); }

    this.required = function (b) { inp.required(b); }
    this.valueOf = function () { return inp.valueOf(); }
}

function VstContainer(n) {
    tag.apply(this, [n]);
    this.childOf = function (p) {
        if (!p) { p = "main-content"; }
        document.getElementById(p).appendChild(this.node);
    }
}
function VstControlBox(d, p) {
    VstContainer.apply(this);
    this.addClass("row");

    function get(v, d) {
        return !v ? d : v;
    }

    this.generate = function (parent, cols) {
        this.html(null);

        this.controls = {};
        for (var i in cols) {
            var tg = get(cols[i].tag, "input");
            if (tg == "none") { continue; }

            var cls = "col-" + get(cols[i].cls, "12");
            var div = this.child(new tag().addClass(cls));
            div = div.child(new tag().addClass("form-group"));

            var c = div.child(new window[tg]);
            var val;
            for (var key in cols[i]) {

                if (key == "tag" || key == "cls") {
                    continue;
                }
                if (key == "value") {
                    val = cols[i][key];
                    continue;
                }

                var f = c[key];
                var v = cols[i][key];
                if (!f) {
                    c.attr(key, v);
                } else {
                    c[key](v);
                }

                if (key == "name") { this.controls[v] = c; }
            }

            if (!cols[i].caption) { c.caption(cols[i].name); }
            if (val) { c.value(val); }
        }

        this.childOf(parent);

        return this;
    }
    this.value = function (data) {

        if (data) {
            for (var key in this.controls) {
                this.controls[key].value(data[key]);
            }
        }

        return this;
    }
    this.valueOf = function () {
        var obj = {};
        for (var key in this.controls) {
            var v = this.controls[key].valueOf();
            if (v == null) {
                return null;
            }

            if (v != "") { obj[key] = v; }
        }
        return obj;
    }

    if (d) { this.generate(p, d); }
}

function VstTable(d, p, u, a) {

    if (u == null) { u = 7; }

    VstContainer.apply(this);
    this.addClass("progress-table-wrap");
    var table = this.child(new tag()).addClass("progress-table");

    var head = table.child(new tag().addClass("table-head"));

    var self = this;
    var rowsData;
    var columns;

    function createAct(div, name) {
        div.child(new tag()).addClass("btn " + name).html("<i class='fa fa-" + name + "'></i>").event("click", function (n) {
            if (n.tagName != "DIV") { n = n.parentElement; }
            var id = n.parentElement.id;
            var a = n.className.split(' ');

            self[a[a.length - 1]](n.parentElement.getAttribute("data-index"));
        });
    }
    this.generate = function () {
        columns = {};
        head.html(null);

        var cols = d;
        for (var i in cols) {
            var col = cols[i];
            var n = col.name;
            var c = col.caption;
            if (!c) { col.caption = c = n; }

            var td = head.child(new tag().addClass(n).html(c));
            columns[n] = col;
        }
        var act = head.child(new tag().addClass("action"));
        if (u & 1) {
            createAct(act, "plus");
        }

        this.childOf(p);
        return this;
    }

    this.value = function (data) {
        rowsData = data;
        for (var i in data) {
            var tr = table.child(new tag().addClass("table-row"));
            for (var k in columns) {
                var v = data[i][k];
                if (!v) { v = ""; }
                tr.child(new tag()).addClass(k).html(v);
            }
            var act = tr.child(new tag().addClass("action")).data("index", i);

            if (a) {
                var it = a.split(',');
                for (var k = 0; k < it.length; k++) {
                    var s = it[k].trim();
                    createAct(act, s);
                }
            }

            if (u & 2) {
                createAct(act, "edit");
            }
            if (u & 4) {
                createAct(act, "minus");
            }
        }

        return this;
    }

    this.autoFormAction = function (c, i) { return "/" + c + "?id=" + id; }
    this.autoUpdate = function (v) {
        var url = window.location.pathname.split('/');
        var controllerName = url[1];

        var a = this.autoFormAction(controllerName, v.id);
        var frm = this.child(new tag("form")).attr("action", a).attr("method", "post").show(false);
        frm.child(new input().name("action").value(v.action));

        if (v.id) {
            frm.child(new input().name("objectid").value(v.id));
        }
        if (v.value) {
            frm.child(new input().name("value").value(JSON.stringify(v.value)));
        }
        frm.node.submit();
    }
    this.plus = function () {
        new VstModal("Thêm mới", function (b, a) {
            var cb = new VstControlBox(d);
            b.child(cb);
            a.event("click", function () {
                self.autoUpdate({ value: cb.valueOf(), action: "add" });
            });
        }, null, "plus");
    }
    this.edit = function (k) {
        new VstModal("Cập nhật", function (b, a) {
            var cb = new VstControlBox(d).value(rowsData[k]);
            b.child(cb);
            a.event("click", function () {
                self.autoUpdate({ value: cb.valueOf(), action: "edit", id: rowsData[k].Id });
            });

        }, null, "edit");
    }
    this.minus = function (k) {
        new VstModal("Xóa", function (b, a) {
            var dl = new tag("dl");
            var row = rowsData[k];

            for (var key in columns) {
                dl.child(new tag("dt").html(columns[key].caption));
                dl.child(new tag("dd").html(row[key]));
            }
            b.child(dl);
            a.addClass("btn-danger");
            a.event("click", function () {
                self.autoUpdate({ action: "delete", id: row.Id });
            });

        }, null, "del");
    }

    this.rowAt = function (k) {
        return rowsData[k];
    }

    if (d) { this.generate(p, d); }
}

function VstModal(title, callback, cls, id) {
    tag.apply(this);

    if (!id) id = "vstModal";
    if (document.getElementById(id)) { document.getElementById(id).remove(); }

    if (!cls) cls = "primary";

    var content = this.id(id).addClass("modal fade").attr("role", "dialog")
        .child(new tag().addClass("modal-dialog"))
        .child(new tag().addClass("modal-content"));

    var header = content.child(new tag().addClass("modal-header")).html("<h4 class='modal-title'>" + title + "</h4><button type='button' class='close' data-dismiss='modal'>&times;</button>");
    var body = content.child(new tag().addClass("modal-body"));
    var footer = content.child(new tag().addClass("modal-footer"));
    var accept = footer.child(new tag("button").addClass("btn btn-" + cls).html("OK")).data("dismiss", "modal");
    footer.child(new tag("button").addClass("btn btn-default").html("Cancel")).id("toggle" + id).data("target", '#' + id).data("toggle", "modal").event("click", function () {
        var n = document.getElementById(id);
        if (n.className.indexOf("show") > 0) {
            var t = setTimeout(function () { n.remove(); }, 500);
        }
    });

    document.body.appendChild(this.node);
    callback(body, accept);

    document.getElementById("toggle" + id).click();
}

function VstSubmitForm(a, d, p, o) {
    VstContainer.apply(this, ["form"]);
    this.attr("action", a)
        .attr("method", "post")
        .id("main-form");
    this.childOf(p);

    var box = new VstControlBox(d, "main-form").value(o);
    box.next(new tag()).addClass("footer form-group mt-3")
        .html("<button class='btn btn-primary' id='submit-button'>SUBMIT</button>");

    this.inputs = box.controls;

}