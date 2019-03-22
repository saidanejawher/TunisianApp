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
        //var initializeClientsTable = function () {
        //    $("table#tableClients").DataTable({
        //        "processing": "true",
        //        "language": {
        //            "emptyTable": "Aucune donnée disponible",
        //            "info": "Affichage de _START_ à _END_ de _TOTAL_ entrées",
        //            "infoEmpty": "Affichage de 0 à 0 des 0 entrées",
        //            "infoFiltered": "(filtrées de _MAX_ entrées en total)",
        //            "infoPostFix": "",
        //            "thousands": ",",
        //            "lengthMenu": "Afficher _MENU_ entrées",
        //            "loadingRecords": "Chargement...",
        //            "processing": "Traitement...",
        //            "search": "Rechercher:",
        //            "zeroRecords": "Aucun résultat trouvé",
        //            "paginate": {
        //                "first": "Premier",
        //                "last": "Dernier",
        //                "next": "Suivant",
        //                "previous": "Précédent"
        //            },
        //            "aria": {
        //                "sortAscending": ": Activer le tri ascendant de la colonne",
        //                "sortDescending": ": Activer le tri descendant de la colonne"
        //            }
        //        },
        //        "order": [[2, "asc"]],
        //        "paging": true,
        //        "ordering": true,
        //        "searching": true,
        //        "info": true,
        //    });
        //};
        var tabletableClients = function () {
            $("table#tableClients").DataTable({
                "ajax": {
                    "url": $("#approot").val() + "Home/ListClientsJson",
                    "type": "GET",
                    "data": function (d) {
                        //d.idOdc = window.$("#idOuvertureCompte").val();
                        //d.idTiers = window.$("#IdTiers").val();
                    }
                },
                "processing": true,
                "autoWidth": false,
                "language": {
                    "emptyTable": "Aucun document disponible",
                    "info": "",
                    "infoEmpty": "",
                    "loadingRecords": " Chargement...",
                    "processing": "  Traitement..."//,
                },
                "paging": false,
                "ordering": false,
                "searching": false,
                "ColumnDefs": [
                    { "width": "926px", "bSortable": false, className: "text-center" },
                    { "width": "137px", "bSortable": false, className: "text-center" }
                ],
                "aaSorting": [],
                "columns": [

                    {
                        orderable: false,
                        render: function (data, type, row, meta) {
                            row.Nom;
                        }
                    },
                    {
                        "className": "text-center",
                        "data": "DocTitle",
                        "render": function (data, type, row) {
                            //data = row.StatutOcr;


                            return row.Prenom;
                        }
                    },
                    {
                        "className": "text-center",
                        orderable: false,
                        render: function (data, type, row, meta) {
                            return row.Age;
                        }
                    },

                ],

                "fnDrawCallback": function () {
                    $(".infoTooltip").tooltip({
                        content: function () {
                            return $(this).prop("title");
                        }
                    });
                    setTimeout(function () { $('table#tableClients').css('width', '100%'); }, 30);
                    var table = $("table#tableClients").DataTable();
                }
            });
        }
        //---------
      
        /***********************
        * ProspectClass : Initialize the class
        **********************/
        this.initializeClientsClass = function () {

            initializeEvent();
            //initializeClientsTable();
            tabletableClients();
          
        };
    }
    // ===================
    var Clients = new Clients();
    Clients.initializeClientsClass();
});



