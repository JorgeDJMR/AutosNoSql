namespace AutosNoSql;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using MongoDB.Driver;

public partial class listaAutos : ContentPage
{

    private IMongoCollection<Auto> _autosCollection;
    private MongoClient _mongoClient;
    private readonly string _databaseName = "AutosEnMongo";
    private readonly string _connectionString = "mongodb://192.168.84.33:27017";

    // Inicializar la conexión con la base de datos MongoDB
    private void InitializeDatabase()
    {
        _mongoClient = new MongoClient(_connectionString);
        var database = _mongoClient.GetDatabase(_databaseName);
        _autosCollection = database.GetCollection<Auto>("autos");
    }

    public listaAutos()
	{
		InitializeComponent();
        this.Title = "Autos"; // Establece el título de la página

        // Configura el icono de la página
        this.IconImageSource = "autos.png";

        InitializeDatabase();
        CargarAutos();

 
    }



    // Método para cargar los autos desde la base de datos y mostrarlos en el ListView
    public async void CargarAutos()
    {
        var filter = Builders<Auto>.Filter.Eq(a => a.Rentado, false);//Me filtra los autos, mostrandome en listaAutos solo los autos que no estan rentados(false)
        var autos = await _autosCollection.Find(filter).ToListAsync();
        listViewAutos.ItemsSource = autos;
    }

    //Pasar datos del elemento seleccionado a otra pesta;a
    private void listViewAutos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        // Verificar si se seleccionó un elemento
        if (e.SelectedItem != null && e.SelectedItem is Auto autoSeleccionado)
        {
            // Obtener la instancia de la página CrearRenta
            var mainPage = Application.Current.MainPage as MainPage;
            var crearRentaPage = mainPage?.Children[2] as CrearRenta;

            crearRentaPage.PasarDatosEntry(autoSeleccionado);
        }


    }





}