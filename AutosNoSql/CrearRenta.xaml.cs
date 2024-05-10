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
        this.Title = "Rentar"; // Establece el título de la página

        // Configura el icono de la página
        this.IconImageSource = "renta.png";


        // Configurar la conexión con la base de datos
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
        // Verificar si todos los campos están llenos
        if (string.IsNullOrWhiteSpace(EPlacas.Text) ||
            string.IsNullOrWhiteSpace(ECliente.Text) ||
            string.IsNullOrWhiteSpace(EPrecioPorDia.Text) ||
            string.IsNullOrWhiteSpace(ETelefono.Text) ||
            string.IsNullOrWhiteSpace(EPrecioTotal.Text))
        {
            // Mostrar un mensaje de advertencia si falta algún campo
            DisplayAlert("Advertencia", "Por favor, completa todos los campos antes de continuar.", "OK");
            return; // Salir del método sin ejecutar la actualización
        }

        // Crear el filtro para encontrar el auto con la placa especificada
        var filter = Builders<Auto>.Filter.Eq(a => a.Placa, placa);

        // Crear la actualización para cambiar el estado de Rentado a true
        var update = Builders<Auto>.Update.Set(a => a.Rentado, true);

        // Ejecutar la actualización
        var result = _collection.UpdateOne(filter, update);

        // Verificar si se realizó la actualización correctamente
        if (result.ModifiedCount > 0)
        {
            // La actualización fue exitosa
            DisplayAlert("Aviso", "El auto se ha rentado correctamente.", "OK");
            LimpiarCampos();
            var mainPage = Application.Current.MainPage as MainPage;
            var crearRentaPage = mainPage?.Children[1] as listaAutos;
            crearRentaPage.CargarAutos();
        }
        else
        {
            // No se encontró ningún auto con la placa especificada
            DisplayAlert("Aviso", "No se encontró ningún auto con la placa especificada.", "OK");
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
        // Limpia los demás campos de la página según sea necesario
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

        // Verificar si EPrecioPorDia.Text no es nulo ni está vacío
        if (!string.IsNullOrEmpty(EPrecioPorDia.Text))
        {
            // Intentar convertir el valor de EPrecioPorDia.Text a un número
            if (int.TryParse(EPrecioPorDia.Text, out int precioPorDia))
            {
                // Calcular el precio total y asignarlo al Entry correspondiente
                int precioTotal = precioPorDia * diasRenta;
                EPrecioTotal.Text = precioTotal.ToString();
            }
            else
            {
                // Si no se puede convertir a un número, mostrar un mensaje de error o manejar la situación según sea necesario
                Console.WriteLine("El valor de EPrecioPorDia no es un número válido.");
            }
        }
        else
        {
            // Si EPrecioPorDia.Text es nulo o está vacío, mostrar un mensaje de error o manejar la situación según sea necesario
            Console.WriteLine("El valor de EPrecioPorDia es nulo o está vacío.");
        }

    }

    private void FechaEntrega_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        DateTime inicio = FechaSalida.Date;
        DateTime entrega = FechaEntrega.Date;
        int diasRenta = (int)(entrega - inicio).TotalDays;

        // Verificar si EPrecioPorDia.Text no es nulo ni está vacío
        if (!string.IsNullOrEmpty(EPrecioPorDia.Text))
        {
            // Intentar convertir el valor de EPrecioPorDia.Text a un número
            if (int.TryParse(EPrecioPorDia.Text, out int precioPorDia))
            {
                // Calcular el precio total y asignarlo al Entry correspondiente
                int precioTotal = precioPorDia * diasRenta;
                EPrecioTotal.Text = precioTotal.ToString();
            }
            else
            {
                // Si no se puede convertir a un número, mostrar un mensaje de error o manejar la situación según sea necesario
                Console.WriteLine("El valor de EPrecioPorDia no es un número válido.");
            }
        }
        else
        {
            // Si EPrecioPorDia.Text es nulo o está vacío, mostrar un mensaje de error o manejar la situación según sea necesario
            Console.WriteLine("El valor de EPrecioPorDia es nulo o está vacío.");
        }
    }
}