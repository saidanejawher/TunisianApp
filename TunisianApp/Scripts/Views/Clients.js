$(function () {

    var Clients = function () {
        /***********************
       * Prospect : Initialize all events
       **********************/
        var initializeEvent = function () {
            alert("test");
        };

        /***********************
        * Prospect : Initialize DataTables into Prospect Table
        **********************/
      
        //var tabletableClients = function () {
        //    $("table#tableClients").DataTable({
        //        "ajax": {
        //            "url": $("#approot").val() + "Home/ListClientsJson",
        //            "type": "GET",
        //            "data": function (d) {
        //                //d.idOdc = window.$("#idOuvertureCompte").val();
        //                //d.idTiers = window.$("#IdTiers").val();
        //            }
        //        },
        //        "processing": true,
        //        "autoWidth": false,
        //        "language": {
        //            "emptyTable": "Aucun document disponible",
        //            "info": "",
        //            "infoEmpty": "",
        //            "loadingRecords": " Chargement...",
        //            "processing": "  Traitement..."//,
        //        },
        //        "paging": false,
        //        "ordering": false,
        //        "searching": false,
        //        "ColumnDefs": [
        //            { "width": "926px", "bSortable": false, className: "text-center" },
        //            { "width": "137px", "bSortable": false, className: "text-center" }
        //        ],
        //        "aaSorting": [],
        //        "columns": [

        //            {
        //                orderable: false,
        //                render: function (data, type, row, meta) {
        //                    row.Nom;
        //                }
        //            },
        //            {
        //                "className": "text-center",
        //                "data": "DocTitle",
        //                "render": function (data, type, row) {
        //                    //data = row.StatutOcr;


        //                    return row.Prenom;
        //                }
        //            },
        //            {
        //                "className": "text-center",
        //                orderable: false,
        //                render: function (data, type, row, meta) {
        //                    return row.Age;
        //                }
        //            },

        //        ],

        //        "fnDrawCallback": function () {
        //            $(".infoTooltip").tooltip({
        //                content: function () {
        //                    return $(this).prop("title");
        //                }
        //            });
        //            setTimeout(function () { $('table#tableClients').css('width', '100%'); }, 30);
        //            var table = $("table#tableClients").DataTable();
        //        }
        //    });
        //}
        //---------
        var EditClient = function () {
            $("#EditCient").on("click",
                function (e) {
                    e.preventDefault();
                    var $form = $(this).closest("form");
                    var data = $form.serialize();
                    
                        $.post($form.attr("action"),
                            data,
                            function (data, status) {
                                if (data.success === false) {
                                    toastr.error('Erreur lors de la création du client.');
                                }
                                else if (data.success === true) {
                                    $("[id^=modalEdit]").modal('hide');
                                    toastr.success('Client modifié avec succès !');
                                    setTimeout(function () {
                                        location.reload();
                                    },
                                        500);
                                }
                            });
                  
                });
        }
        /***********************
        * ProspectClass : Initialize the class
        **********************/
        this.initializeClientsClass = function () {

            initializeEvent();
            //initializeClientsTable();
            //tabletableClients();
            EditClient();
        };
    }
    // ===================
    var Client = new Clients();
    Client.initializeClientsClass();
});



