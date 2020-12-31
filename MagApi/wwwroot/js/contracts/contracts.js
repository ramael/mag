﻿define([], function () {
    function Warehouse(id, name, description, notes, areas) {
        this.id = id;
        this.name = name;
        this.description = description;
        this.notes = notes;
        this.areas = areas;
    }

    function Area(id, name, description, notes, locations) {
        this.id = id;
        this.name = name;
        this.description = description;
        this.notes = notes;
        this.locations = locations;
    }

    function Location(id, name, description, notes) {
        this.id = id;
        this.name = name;
        this.description = description;
        this.notes = notes;
    }

    function Cart(id, serialnumber, status) {
        this.id = id;
        this.serialnumber = serialnumber;
        this.status = status;

        this.getStatusLabel = function() {
            switch (this.status) {
                case 0:
                    return "Available";
                case 1:
                    return "NotAvailable";
                case 2:
                    return "UnderRepair";
                case 3:
                    return "Destroyed";
                default:
                    return "Unknown";
            }
        }
    }

    function Component(id, code, description, notes) {
        this.id = id;
        this.code = code;
        this.description = description;
        this.notes = notes;
    }

    function LoadedCart(id, year, progressive, locationid, location, cartid, cart, description, datein, dateout, loadedcartdetails) {
        this.id = id;
        this.year = year;
        this.progressive = progressive;
        this.locationid = locationid;
        this.location = location;
        this.cartid = cartid;
        this.cart = cart;
        this.description = description;
        this.datein = datein;
        this.dateout = dateout;
        this.loadedcartdetails = loadedcartdetails;
    }

    function LoadedCartDetail(id, loadedcartid, componentid, component, notes) {
        this.id = id;
        this.loadedcartid = loadedcartid;
        this.componentid = componentid;
        this.component = component;
        this.notes = notes;
    }

    function LoginRequest(username, password) {
        this.username = username;
        this.password = password;
    }

    function LoginResponse(username, firstname, lastname, roles, token) {
        this.username = username;
        this.firstname = firstname;
        this.lastname = lastname;
        this.roles = roles;
        this.token = token;
    }

    function Stock(componentcode, componentdescription, componentqty, details) {
        this.componentcode = componentcode;
        this.componentdescription = componentdescription;
        this.componentqty = componentqty;
        this.details = details;
    }

    function StockDetail(serialnumber, areaname, locationname) {
        this.serialnumber = serialnumber;
        this.areaname = areaname;
        this.locationname = locationname;
    }

    return {
        warehouse: Warehouse,
        area: Area,
        location: Location,
        cart: Cart,
        component: Component,
        loadedcart: LoadedCart,
        loadedcartdetail: LoadedCartDetail,
        loginrequest: LoginRequest,
        loginresponse: LoginResponse,
        stock: Stock,
        stockdetail: StockDetail
    }
});
