using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using ActividadesAcademicas.Modelos;
using ActividadesAcademicas.Negocio;
using ActividadesAcademicas.Web.Presenters;

namespace ActividadesAcademicas.Web
{
    /// <summary>
    /// Vista (MVP). Solo lee/escribe controles y delega toda accion al Presenter.
    /// No contiene SQL ni reglas de negocio.
    /// </summary>
    public partial class Actividades : Page, IActividadView
    {
        private ActividadPresenter _presenter;

        protected void Page_Load(object sender, EventArgs e)
        {
            var cs = ConfigurationManager.ConnectionStrings["ActividadesDB"];
            if (cs == null || string.IsNullOrWhiteSpace(cs.ConnectionString))
            {
                MostrarMensaje("La cadena de conexión 'ActividadesDB' no está configurada en Web.config.", false);
                return;
            }

            _presenter = new ActividadPresenter(this, ServiceFactory.CrearActividadService(cs.ConnectionString));
            if (!IsPostBack)
            {
                MostrarTextosIniciales();
                _presenter.Inicializar();
            }
        }

        // ---------------- Eventos -> Presenter ----------------
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (_presenter == null) return;
            gvActividades.PageIndex = 0;
            _presenter.Guardar();
        }

        protected void btnLimpiar_Click(object sender, EventArgs e) { LimpiarFormulario(); OcultarMensajes(); }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            if (_presenter == null) return;
            gvActividades.PageIndex = 0;
            _presenter.Buscar();
        }

        protected void btnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            if (_presenter == null) return;
            gvActividades.PageIndex = 0;
            _presenter.LimpiarFiltros();
        }

        protected void gvActividades_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (_presenter == null) return;
            gvActividades.PageIndex = e.NewPageIndex;
            _presenter.Buscar();
        }

        protected void gvActividades_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_presenter == null) return;
            int id = Convert.ToInt32(gvActividades.DataKeys[gvActividades.SelectedIndex].Value);
            _presenter.VerDetalle(id);
        }

        // ---------------- IActividadView: entradas ----------------
        public string Titulo { get { return txtTitulo.Text; } }
        public string Descripcion { get { return txtDescripcion.Text; } }
        public string TipoId { get { return ddlTipo.SelectedValue; } }
        public string FechaInicio { get { return txtFechaInicio.Text; } }
        public string FechaFin { get { return txtFechaFin.Text; } }
        public string HoraInicio { get { return txtHoraInicio.Text; } }
        public string HoraFin { get { return txtHoraFin.Text; } }
        public string Modalidad { get { return ddlModalidad.SelectedValue; } }
        public string Lugar { get { return txtLugar.Text; } }
        public string Cupo { get { return txtCupo.Text; } }
        public string Estado { get { return ddlEstado.SelectedValue; } }

        public string FiltroTexto { get { return txtBuscar.Text; } }
        public string FiltroTipoId { get { return ddlFiltroTipo.SelectedValue; } }
        public string FiltroEstado { get { return ddlFiltroEstado.SelectedValue; } }
        public string FiltroFecha { get { return txtFiltroFecha.Text; } }

        // ---------------- IActividadView: salidas ----------------
        public void MostrarTipos(IEnumerable<TipoActividad> tipos)
        {
            foreach (var t in tipos)
            {
                ddlTipo.Items.Add(new ListItem(t.Nombre, t.TipoId.ToString()));
                ddlFiltroTipo.Items.Add(new ListItem(t.Nombre, t.TipoId.ToString()));
            }
        }

        public void MostrarActividades(IEnumerable<Actividad> actividades)
        {
            var lista = actividades.ToList();
            gvActividades.SelectedIndex = -1;
            gvActividades.DataSource = lista;
            gvActividades.DataBind();

            if (lista.Count == 0)
            {
                lblPaginacion.Text = "No hay registros para mostrar.";
            }
            else
            {
                int desde = gvActividades.PageIndex * gvActividades.PageSize + 1;
                int hasta = Math.Min(desde + gvActividades.PageSize - 1, lista.Count);
                lblPaginacion.Text = string.Format("Mostrando {0} a {1} de {2} registros", desde, hasta, lista.Count);
            }
        }

        public void MostrarResumen(int total, int inscritos, int proximas, int finalizadas)
        {
            lblTotal.Text = total.ToString();
            lblInscritosTotal.Text = inscritos.ToString();
            lblProximas.Text = proximas.ToString();
            lblFinalizadas.Text = finalizadas.ToString();
        }

        public void MostrarDetalle(Actividad a)
        {
            var d = new StringBuilder();
            d.Append(Fila("Título:", H(a.Titulo)));
            d.Append(Fila("Tipo:", H(a.TipoNombre)));
            d.Append(Fila("Descripción:", H(a.Descripcion)));
            d.Append(Fila("Fecha:", a.FechaInicio.ToString("dd/MM/yyyy") + " - " + a.FechaFin.ToString("dd/MM/yyyy")));
            d.Append(Fila("Hora:", a.HoraInicio.ToString(@"hh\:mm") + " - " + a.HoraFin.ToString(@"hh\:mm")));
            d.Append(Fila("Modalidad:", H(TextoModalidad(a.Modalidad))));
            d.Append(Fila("Lugar:", H(a.Lugar)));
            d.Append(Fila("Cupos:", a.Cupo.ToString()));
            d.Append(Fila("Estado:", Badge(a.Estado)));
            litDetalle.Text = d.ToString();

            int disponibles = Math.Max(0, a.Cupo - a.Inscritos);
            var v = new StringBuilder();
            v.Append("<h3>").Append(H(a.Titulo)).Append("</h3>");
            v.Append("<div class='chips'><span class='chip'>").Append(H(a.TipoNombre)).Append("</span>").Append(Badge(a.Estado)).Append("</div>");
            v.Append("<p>&#128197; ").Append(a.FechaInicio.ToString("dd/MM/yyyy")).Append(" - ").Append(a.FechaFin.ToString("dd/MM/yyyy")).Append("</p>");
            v.Append("<p>&#128343; ").Append(a.HoraInicio.ToString(@"hh\:mm")).Append(" - ").Append(a.HoraFin.ToString(@"hh\:mm")).Append("</p>");
            v.Append("<p>&#128205; ").Append(H(a.Lugar)).Append(" (").Append(H(TextoModalidad(a.Modalidad))).Append(")</p>");
            v.Append("<p>&#128101; Cupos disponibles: ").Append(disponibles).Append(" de ").Append(a.Cupo).Append("</p>");
            v.Append("<p class='desc'>").Append(H(a.Descripcion)).Append("</p>");
            litVista.Text = v.ToString();
        }

        public void MostrarInscritos(IEnumerable<Inscrito> inscritos)
        {
            var lista = inscritos.ToList();
            litTituloInscritos.Text = "Estudiantes Inscritos (" + lista.Count + ")";
            if (lista.Count == 0)
            {
                litInscritos.Text = "<p class='vacio'>Aún no hay estudiantes inscritos.</p>";
                return;
            }
            var sb = new StringBuilder();
            sb.Append("<table class='mini'><tr><th>#</th><th>Nombre</th><th>Programa</th><th>Fecha inscripción</th></tr>");
            int n = 1;
            foreach (var i in lista)
            {
                sb.Append("<tr><td>").Append(n++).Append("</td><td>").Append(H(i.Nombre)).Append("</td><td>")
                  .Append(H(i.Programa)).Append("</td><td>").Append(i.FechaInscripcion.ToString("dd/MM/yyyy")).Append("</td></tr>");
            }
            sb.Append("</table>");
            litInscritos.Text = sb.ToString();
        }

        public void MostrarMensaje(string mensaje, bool esExito)
        {
            lblMensaje.Text = WebUtility.HtmlEncode(mensaje);
            lblMensaje.CssClass = esExito ? "msg ok" : "msg error";
            lblMensaje.Visible = true;
        }

        public void MostrarErrores(IEnumerable<string> errores)
        {
            blErrores.Items.Clear();
            if (errores != null)
                foreach (var er in errores) blErrores.Items.Add(er);
            blErrores.Visible = blErrores.Items.Count > 0;
        }

        public void LimpiarFormulario()
        {
            txtTitulo.Text = string.Empty;
            txtDescripcion.Text = string.Empty;
            ddlTipo.SelectedIndex = 0;
            ddlModalidad.SelectedIndex = 0;
            txtFechaInicio.Text = string.Empty;
            txtFechaFin.Text = string.Empty;
            txtHoraInicio.Text = string.Empty;
            txtHoraFin.Text = string.Empty;
            txtCupo.Text = string.Empty;
            txtLugar.Text = string.Empty;
            ddlEstado.SelectedIndex = 0;
        }

        public void LimpiarFiltros()
        {
            txtBuscar.Text = string.Empty;
            ddlFiltroTipo.SelectedIndex = 0;
            ddlFiltroEstado.SelectedIndex = 0;
            txtFiltroFecha.Text = string.Empty;
        }

        // ---------------- Utilidades de presentacion ----------------
        protected static string TextoEstado(object estado)
        {
            var e = Convert.ToString(estado);
            return e == "Proxima" ? "Próxima" : e;
        }

        private static string TextoModalidad(string m)
        {
            return m == "Hibrida" ? "Híbrida" : m;
        }

        private static string H(string s)
        {
            return WebUtility.HtmlEncode(s ?? string.Empty);
        }

        private static string Badge(string estado)
        {
            return "<span class='badge " + H(estado) + "'>" + H(TextoEstado(estado)) + "</span>";
        }

        private static string Fila(string etiqueta, string valorHtml)
        {
            return "<div class='fila'><span class='et'>" + etiqueta + "</span><span>" + valorHtml + "</span></div>";
        }

        private void MostrarTextosIniciales()
        {
            litDetalle.Text = "<p class='vacio'>Seleccione una actividad del listado (botón «Ver») para consultar su detalle.</p>";
            litTituloInscritos.Text = "Estudiantes Inscritos (0)";
            litInscritos.Text = "<p class='vacio'>Seleccione una actividad para ver sus inscritos.</p>";
            litVista.Text = "<p class='vacio'>Aquí se mostrará la actividad tal como la ve el estudiante.</p>";
        }

        private void OcultarMensajes()
        {
            lblMensaje.Visible = false;
            blErrores.Visible = false;
        }
    }
}
