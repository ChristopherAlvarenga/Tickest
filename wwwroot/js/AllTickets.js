<script src="~/js/AllTickets.js"></script>

$(document).ready(function () {
    // Abre o modal e carrega as mensagens ao clicar em "Ver mais"
    $(".modal-ticket").click(function () {
        var ticket_id = $(this).attr("ticket-id");
        var url = "/Chat/Mensagens/" + ticket_id;
        $.get(url, function (data) {
            $("#chat-" + ticket_id).html(data);
            $("#chat-" + ticket_id).scrollTop($("#chat-" + ticket_id)[0].scrollHeight);
        });
    });

    // Envia mensagem ao pressionar Enter dentro do modal
    $(".modal").on('keypress', function (e) {
        if (e.which == 13) {
            var ticket_id = $(this).attr("ticket-id");
            $("#enviar-" + ticket_id).click();
        }
    });

    // Abre a caixa de diálogo de anexo ao clicar no ícone de papelclip
    $(".attach_btn").click(function () {
        var ticket_id = $(this).attr("ticket-id");
        $("#attach-" + ticket_id).click();
    });

    // Envia o arquivo selecionado para o servidor
    $(".input-attach").change(function () {
        var id = $(this).attr("ticket-id");
        var file = document.getElementById('attach-' + id).files[0];
        var filename = document.getElementById('attach-' + id).files[0].name;
        var formData = new FormData();
        formData.append("file", file);

        // Realiza o upload do arquivo via AJAX
        $.ajax({
            dataType: "json",
            type: "POST",
            url: "/Chat/Upload",
            cache: false,
            contentType: false,
            processData: false,
            data: formData,
            success: function (retorno) {
                console.log(retorno);
                if (retorno['sucesso']) {
                    var url = retorno['sucesso'];
                    var html = '<a class="help-button col-sm-4 downloadupaddinvoice" download="' + filename + '" href="../' + url + '" title="Baixar anexo" download><i class="icon-download-alt"></i>' + filename + '</a>';
                    var formData = new FormData();
                    formData.append("ticket_id", id);
                    formData.append("msg", html);

                    // Envia mensagem com o link do anexo
                    $.ajax({
                        dataType: "json",
                        type: "POST",
                        url: "/Chat/Enviar",
                        cache: false,
                        contentType: false,
                        processData: false,
                        data: formData,
                        success: function (retorno) {
                            console.log(retorno);
                            if (retorno['sucesso']) {
                                $("#text-" + id).val("");
                                var url = "/Chat/Mensagens/" + id;

                                // Atualiza o chat após o envio da mensagem
                                $.get(url, function (data) {
                                    $("#chat-" + id).html(data);
                                    $("#chat-" + id).scrollTop($("#chat-" + id)[0].scrollHeight);
                                });
                            }
                        }
                    });
                }
            }
        });
    });

    // Envia mensagem de texto
    $(".send_btn").click(function () {
        var formData = new FormData();
        var ticket_id = $(this).attr("ticket-id");
        var msg = $("#text-" + ticket_id).val();
        formData.append("ticket_id", ticket_id);
        formData.append("msg", msg);

        // Envia a mensagem via AJAX
        $.ajax({
            dataType: "json",
            type: "POST",
            url: "/Chat/Enviar",
            cache: false,
            contentType: false,
            processData: false,
            data: formData,
            success: function (retorno) {
                console.log(retorno);
                if (retorno['sucesso']) {
                    $("#text-" + ticket_id).val("");
                    var url = "/Chat/Mensagens/" + ticket_id;

                    // Atualiza o chat após o envio da mensagem
                    $.get(url, function (data) {
                        $("#chat-" + ticket_id).html(data);
                        $("#chat-" + ticket_id).scrollTop($("#chat-" + ticket_id)[0].scrollHeight);
                    });
                }
            }
        });
    });

    // Configuração do DataTable para a tabela de tickets
    $('#Tickets').DataTable({
        "ordering": true,
        "paging": true,
        "searching": true,
        "oLanguage": {
            "sEmptyTable": "Nenhum registro encontrado na tabela",
            "sInfo": "Mostrar _START_ até _END_ de _TOTAL_ registros",
            "sInfoEmpty": "Mostrar 0 até 0 de 0 Registros",
            "sInfoFiltered": "(Filtrar de _MAX_ total registros)",
            "sInfoPostFix": "",
            "sInfoThousands": ".",
            "sLengthMenu": "Mostrar _MENU_ registros por página",
            "sLoadingRecords": "Carregando...",
            "sProcessing": "Processando...",
            "sZeroRecords": "Nenhum registro encontrado",
            "sSearch": "Pesquisar",
            "oPaginate": {
                "sNext": "Próximo",
                "sPrevious": "Anterior",
                "sFirst": "Primeiro",
                "sLast": "Último"
            },
            "oAria": {
                "sSortAscending": ": Ordenar colunas de forma ascendente",
                "sSortDescending": ": Ordenar colunas de forma descendente"
            }
        }
    });

    // Adicionar código JavaScript para atualizar dinamicamente o chat
    @foreach(var ticket in Model.Tickets)
{
    <text>
        setInterval(function () {
                var url = "/Chat/Mensagens/@ticket.Id";
        $.get(url, function (data) {
                    var scroll = $("#chat-@ticket.Id").scrollTop();
        if ($("#chat-@ticket.Id")[0].scrollHeight - $("#chat-@ticket.Id").scrollTop() == $("#chat-@ticket.Id").outerHeight()) {
            $("#chat-@ticket.Id").html(data);
        $("#chat-@ticket.Id").scrollTop($("#chat-@ticket.Id")[0].scrollHeight);
                    } else {
            $("#chat-@ticket.Id").html(data);
        $("#chat-@ticket.Id").scrollTop(scroll);
                    }
                });
            }, 3000);
    </text>
}

// Função para lidar com a submissão do formulário de edição
$(".modal-body form").submit(function (e) {
    e.preventDefault(); // Impede o comportamento padrão do formulário
    var form = $(this);
    var formData = form.serialize();

    $.ajax({
        type: "POST",
        url: form.attr("action"),
        data: formData,
        success: function (data) {
            // Atualiza os dados na modal após a edição
            var ticketId = form.find("input[name='ticketId']").val();
            var titulo = form.find("input[name='titulo']").val();
            var status = form.find("select[name='status']").val();

            // Atualiza os campos na tabela
            $("#titulo-" + ticketId).text(titulo);
            $("#status-" + ticketId).text(status);

            // Fecha a modal após edição
            form.closest(".modal").modal("hide");
        },
        error: function (error) {
            console.error("Erro ao editar ticket:", error);
            // Tratar erro, se necessário
        }
    });
});
});
