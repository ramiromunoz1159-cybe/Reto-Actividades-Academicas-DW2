<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Actividades.aspx.cs" Inherits="ActividadesAcademicas.Web.Actividades" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Registro de Actividades Académicas · UniCampus</title>
    <link href="Content/Default.css" rel="stylesheet" type="text/css" />
</head>
<body>
<form id="form1" runat="server">
<div class="app">

    <!-- ENCABEZADO -->
    <header class="topbar">
        <div class="brand">
            <span class="brand-logo">&#127891;</span>
            <div>
                <strong>UniCampus</strong>
                <small>Sistema de Gestión Académica</small>
            </div>
        </div>
        <div class="user">
            <span class="bell">&#128276;<i>3</i></span>
            <span class="avatar">&#128100;</span>
            <div class="user-info"><strong>María López</strong><small>Administrador</small></div>
        </div>
    </header>

    <!-- MENÚ LATERAL -->
    <aside class="sidebar">
        <nav>
            <span class="nav off"><i>&#8962;</i> Inicio</span>
            <a class="nav activo" href="Actividades.aspx"><i>&#128197;</i> Actividades Académicas</a>
            <span class="nav off"><i>&#9776;</i> Mis Inscripciones</span>
            <span class="nav off"><i>&#128202;</i> Reportes</span>
            <span class="nav off"><i>&#128100;</i> Perfil</span>
        </nav>
        <span class="nav off salir"><i>&#10162;</i> Cerrar sesión</span>
    </aside>

    <!-- CONTENIDO -->
    <main class="contenido">

        <section class="hero">
            <h1>Registro de Actividades Académicas</h1>
            <p>Administra, registra y consulta las actividades académicas de la institución.</p>
            <div class="stats">
                <div class="stat azul"><span class="ico">&#128197;</span><div><b><asp:Label ID="lblTotal" runat="server" Text="0" /></b><small>Total de actividades</small></div></div>
                <div class="stat verde"><span class="ico">&#128101;</span><div><b><asp:Label ID="lblInscritosTotal" runat="server" Text="0" /></b><small>Estudiantes inscritos</small></div></div>
                <div class="stat amarillo"><span class="ico">&#9200;</span><div><b><asp:Label ID="lblProximas" runat="server" Text="0" /></b><small>Actividades próximas</small></div></div>
                <div class="stat rojo"><span class="ico">&#10004;</span><div><b><asp:Label ID="lblFinalizadas" runat="server" Text="0" /></b><small>Actividades finalizadas</small></div></div>
            </div>
        </section>

        <asp:Label ID="lblMensaje" runat="server" Visible="false" />
        <asp:BulletedList ID="blErrores" runat="server" CssClass="errores" Visible="false" />

        <div class="fila-principal">

            <!-- FORMULARIO -->
            <section class="card">
                <h2 class="card-titulo"><span class="mas">+</span> Nueva Actividad Académica</h2>
                <div class="card-cuerpo">
                    <div class="f-grid g2">
                        <div>
                            <label for="txtTitulo">Título de la actividad <span class="req">*</span></label>
                            <asp:TextBox ID="txtTitulo" runat="server" MaxLength="150" />
                        </div>
                        <div>
                            <label for="ddlTipo">Tipo de actividad <span class="req">*</span></label>
                            <asp:DropDownList ID="ddlTipo" runat="server" AppendDataBoundItems="true">
                                <asp:ListItem Value="" Text="Seleccione..." />
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="f-grid g-desc">
                        <div>
                            <label for="txtDescripcion">Descripción <span class="req">*</span></label>
                            <asp:TextBox ID="txtDescripcion" runat="server" TextMode="MultiLine" Rows="3" MaxLength="500" />
                        </div>
                        <div>
                            <label for="txtFechaInicio">Fecha de inicio <span class="req">*</span></label>
                            <asp:TextBox ID="txtFechaInicio" runat="server" TextMode="Date" />
                        </div>
                        <div>
                            <label for="txtFechaFin">Fecha de fin <span class="req">*</span></label>
                            <asp:TextBox ID="txtFechaFin" runat="server" TextMode="Date" />
                        </div>
                    </div>

                    <div class="f-grid g3">
                        <div>
                            <label for="txtHoraInicio">Hora de inicio <span class="req">*</span></label>
                            <asp:TextBox ID="txtHoraInicio" runat="server" TextMode="Time" />
                        </div>
                        <div>
                            <label for="txtHoraFin">Hora de fin <span class="req">*</span></label>
                            <asp:TextBox ID="txtHoraFin" runat="server" TextMode="Time" />
                        </div>
                        <div>
                            <label for="ddlModalidad">Modalidad <span class="req">*</span></label>
                            <asp:DropDownList ID="ddlModalidad" runat="server">
                                <asp:ListItem Value="Presencial" Text="Presencial" />
                                <asp:ListItem Value="Virtual" Text="Virtual" />
                                <asp:ListItem Value="Hibrida" Text="Híbrida" />
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="f-grid g3">
                        <div>
                            <label for="txtCupo">Cupos disponibles <span class="req">*</span></label>
                            <asp:TextBox ID="txtCupo" runat="server" MaxLength="6" />
                        </div>
                        <div>
                            <label for="txtLugar">Lugar <span class="req">*</span></label>
                            <asp:TextBox ID="txtLugar" runat="server" MaxLength="150" />
                        </div>
                        <div>
                            <label for="ddlEstado">Estado <span class="req">*</span></label>
                            <asp:DropDownList ID="ddlEstado" runat="server">
                                <asp:ListItem Value="Activa" Text="Activa" />
                                <asp:ListItem Value="Proxima" Text="Próxima" />
                                <asp:ListItem Value="Finalizada" Text="Finalizada" />
                                <asp:ListItem Value="Inactiva" Text="Inactiva" />
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="acciones">
                        <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn" OnClick="btnGuardar_Click" />
                        <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="btn sec" OnClick="btnLimpiar_Click" />
                    </div>
                </div>
            </section>

            <!-- LISTADO -->
            <section class="card">
                <h2 class="card-titulo"><span class="lista-ico">&#9776;</span> Listado de Actividades Académicas</h2>
                <div class="card-cuerpo">
                    <div class="filtros">
                        <asp:TextBox ID="txtBuscar" runat="server" placeholder="Buscar actividad..." />
                        <asp:DropDownList ID="ddlFiltroTipo" runat="server" AppendDataBoundItems="true">
                            <asp:ListItem Value="" Text="Todos los tipos" />
                        </asp:DropDownList>
                        <asp:DropDownList ID="ddlFiltroEstado" runat="server">
                            <asp:ListItem Value="" Text="Todos los estados" />
                            <asp:ListItem Value="Activa" Text="Activa" />
                            <asp:ListItem Value="Proxima" Text="Próxima" />
                            <asp:ListItem Value="Finalizada" Text="Finalizada" />
                            <asp:ListItem Value="Inactiva" Text="Inactiva" />
                        </asp:DropDownList>
                        <asp:TextBox ID="txtFiltroFecha" runat="server" TextMode="Date" />
                    </div>
                    <div class="acciones sm">
                        <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btn" OnClick="btnBuscar_Click" />
                        <asp:Button ID="btnLimpiarFiltros" runat="server" Text="Limpiar filtros" CssClass="btn sec" OnClick="btnLimpiarFiltros_Click" />
                    </div>

                    <asp:GridView ID="gvActividades" runat="server" CssClass="tabla" AutoGenerateColumns="false"
                        DataKeyNames="ActividadId" GridLines="None" EmptyDataText="No hay actividades para mostrar."
                        AllowPaging="true" PageSize="5"
                        OnPageIndexChanging="gvActividades_PageIndexChanging"
                        OnSelectedIndexChanged="gvActividades_SelectedIndexChanged">
                        <PagerSettings Mode="Numeric" />
                        <PagerStyle CssClass="pager" />
                        <SelectedRowStyle CssClass="sel" />
                        <Columns>
                            <asp:TemplateField HeaderText="#">
                                <ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Titulo" HeaderText="Título" />
                            <asp:BoundField DataField="TipoNombre" HeaderText="Tipo" />
                            <asp:BoundField DataField="FechaInicio" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                            <asp:BoundField DataField="Cupo" HeaderText="Cupos" />
                            <asp:TemplateField HeaderText="Estado">
                                <ItemTemplate><span class='badge <%# Eval("Estado") %>'><%# TextoEstado(Eval("Estado")) %></span></ItemTemplate>
                            </asp:TemplateField>
                            <asp:CommandField ShowSelectButton="true" SelectText="Ver" HeaderText="Acciones" ControlStyle-CssClass="btn-ver" />
                        </Columns>
                    </asp:GridView>
                    <div class="pie-tabla"><asp:Label ID="lblPaginacion" runat="server" /></div>
                </div>
            </section>
        </div>

        <!-- PANELES INFERIORES -->
        <div class="fila-inferior">
            <section class="card">
                <h2 class="card-titulo"><span class="lista-ico">&#128196;</span> Detalles de la Actividad</h2>
                <div class="card-cuerpo detalle"><asp:Literal ID="litDetalle" runat="server" /></div>
            </section>

            <section class="card">
                <h2 class="card-titulo"><span class="lista-ico">&#128101;</span> <asp:Literal ID="litTituloInscritos" runat="server" /></h2>
                <div class="card-cuerpo"><asp:Literal ID="litInscritos" runat="server" /></div>
            </section>

            <section class="card">
                <h2 class="card-titulo"><span class="lista-ico">&#128197;</span> Vista de la Actividad (como la ve el estudiante)</h2>
                <div class="card-cuerpo vista"><asp:Literal ID="litVista" runat="server" /></div>
            </section>
        </div>

    </main>
</div>
</form>
</body>
</html>
