$(function () {
    $("#sortable1, #sortable2").sortable({
        connectWith: ".connectedSortable",
        update: function (event, ui) {
            $("#sortable1 li").each(function (i) {
                let self = $(this);
                self.attr('id', i);

                let input = self.find("input");
                input.attr('class', "ConnectedDepartment");
                input.attr('name', "ConnectedSubdepartmentsNames[" + i + "]");
            });
            $("#sortable2 li").each(function (i) {
                let self = $(this);
                self.attr('id', i);

                let input = self.find("input");
                input.attr('class', "DisconnectedDepartment");
                input.attr('name', "DisconnectedSubdepartmentsNames[" + i + "]");
            });
        }
    }).disableSelection();
});