namespace AutosNoSql;
using Microsoft.Maui.Controls;
using MongoDB.Bson;
using MongoDB.Driver;
using System;

public partial class CrearRenta : ContentPage
{
    private IMongoCollection<Auto> _collection;

    public CrearRenta()
	{
		InitializeComponent();
        this.Title = "Rentar"; // Establece el t�tulo de la p�gina

        // Configura el icono de la p�gina
        this.IconImageSource = "renta.png";


        // Configurar la conexi�n con la base de datos
        var mongoClient = new MongoClient("mongodb://192.168.84.33:27017");
        var database = mongoClient.GetDatabase("AutosEnMongo");
        _collection = database.GetCollection<Auto>("autos");

    }



    public void PasarDatosEntry(Auto DatosLista)
    {
        EPlacas.Text = DatosLista.Placa;
        EPrecioPorDia.Text = DatosLista.PrecioPorDia.ToString();
    }

    public void ActualizarEstadoRentado(string placa)
    {
        // Verificar si todos los campos est�n llenos
        if (string.IsNullOrWhiteSpace(EPlacas.Text) ||
            string.IsNullOrWhiteSpace(ECliente.Text) ||
            string.IsNullOrWhiteSpace(EPrecioPorDia.Text) ||
            string.IsNullOrWhiteSpace(ETelefono.Text) ||
            string.IsNullOrWhiteSpace(EPrecioTotal.Text))
        {
            // Mostrar un mensaje de advertencia si falta alg�n campo
            DisplayAlert("Advertencia", "Por favor, completa todos los campos antes de continuar.", "OK");
            return; // Salir del m�todo sin ejecutar la actualizaci�n
        }

        // Crear el filtro para encontrar el auto con la placa especificada
        var filter = Builders<Auto>.Filter.Eq(a => a.Placa, placa);

        // Crear la actualizaci�n para cambiar el estado de Rentado a true
        var update = Builders<Auto>.Update.Set(a => a.Rentado, true);

        // Ejecutar la actualizaci�n
        var result = _collection.UpdateOne(filter, update);

        // Verificar si se realiz� la actualizaci�n correctamente
        if (result.ModifiedCount > 0)
        {
            // La actualizaci�n fue exitosa
            DisplayAlert("Aviso", "El auto se ha rentado correctamente.", "OK");
            LimpiarCampos();
            var mainPage = Application.Current.MainPage as MainPage;
            var crearRentaPage = mainPage?.Children[1] as listaAutos;
            crearRentaPage.CargarAutos();
        }
        else
        {
            // No se encontr� ning�n auto con la placa especificada
            DisplayAlert("Aviso", "No se encontr� ning�n auto con la placa especificada.", "OK");
        }
    }



    private void btnCerrar_Clicked(object sender, EventArgs e)
    {

    }

    private void LimpiarCampos()
    {
        EPlacas.Text = string.Empty;
        EPrecioPorDia.Text = string.Empty;
        EPrecioTotal.Text = string.Empty;
        FechaSalida.Date = DateTime.Today;
        FechaEntrega.Date = DateTime.Today;
        ETelefono.Text = string.Empty;
        ECliente.Text = string.Empty;
        // Limpia los dem�s campos de la p�gina seg�n sea necesario
    }

    private void btnGuardarR_Clicked(object sender, EventArgs e)
    {
        
        ActualizarEstadoRentado(EPlacas.Text);
        
    }


    private void FechaSalida_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        DateTime inicio = FechaSalida.Date;
        DateTime entrega = FechaEntrega.Date;
        int diasRenta = (int)(entrega - inicio).TotalDays ;

        // Verificar si EPrecioPorDia.Text no es nulo ni est� vac�o
        if (!string.IsNullOrEmpty(EPrecioPorDia.Text))
        {
            // Intentar convertir el valor de EPrecioPorDia.Text a un n�mero
            if (int.TryParse(EPrecioPorDia.Text, out int precioPorDia))
            {
                // Calcular el precio total y asignarlo al Entry correspondiente
                int precioTotal = precioPorDia * diasRenta;
                EPrecioTotal.Text = precioTotal.ToString();
            }
            else
            {
                // Si no se puede convertir a un n�mero, mostrar un mensaje de error o manejar la situaci�n seg�n sea necesario
                Console.WriteLine("El valor de EPrecioPorDia no es un n�mero v�lido.");
            }
        }
        else
        {
            // Si EPrecioPorDia.Text es nulo o est� vac�o, mostrar un mensaje de error o manejar la situaci�n seg�n sea necesario
            Console.WriteLine("El valor de EPrecioPorDia es nulo o est� vac�o.");
        }

    }

    private void FechaEntrega_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        DateTime inicio = FechaSalida.Date;
        DateTime entrega = FechaEntrega.Date;
        int diasRenta = (int)(entrega - inicio).TotalDays;

        // Verificar si EPrecioPorDia.Text no es nulo ni est� vac�o
        if (!string.IsNullOrEmpty(EPrecioPorDia.Text))
        {
            // Intentar convertir el valor de EPrecioPorDia.Text a un n�mero
            if (int.TryParse(EPrecioPorDia.Text, out int precioPorDia))
            {
                // Calcular el precio total y asignarlo al Entry correspondiente
                int precioTotal = precioPorDia * diasRenta;
                EPrecioTotal.Text = precioTotal.ToString();
            }
            else
            {
                // Si no se puede convertir a un n�mero, mostrar un mensaje de error o manejar la situaci�n seg�n sea necesario
                Console.WriteLine("El valor de EPrecioPorDia no es un n�mero v�lido.");
            }
        }
        else
        {
            // Si EPrecioPorDia.Text es nulo o est� vac�o, mostrar un mensaje de error o manejar la situaci�n seg�n sea necesario
            Console.WriteLine("El valor de EPrecioPorDia es nulo o est� vac�o.");
        }
    }
}