namespace AutosNoSql;
using MongoDB.Driver;



public partial class RentarAutos : ContentPage
{
    private IMongoCollection<Auto> _autosCollection;
    private MongoClient _mongoClient;
    private readonly string _databaseName = "AutosEnMongo";
    private readonly string _connectionString = "mongodb://192.168.84.33:27017";
    
    // Inicializar la conexi�n con la base de datos MongoDB
    private void InitializeDatabase()
    {
        _mongoClient = new MongoClient(_connectionString);
        var database = _mongoClient.GetDatabase(_databaseName);
        _autosCollection = database.GetCollection<Auto>("autos");
    }

    // M�todo para insertar un nuevo auto en la base de datos
    private async Task InsertarAuto(Auto auto)
    {
        await _autosCollection.InsertOneAsync(auto);
    }

    public RentarAutos()
	{
		InitializeComponent();
        this.Title = "Registrar auto"; // Establece el t�tulo de la p�gina

        // Configura el icono de la p�gina
        this.IconImageSource = "registrarauto.png";
        // Tambi�n puedes configurar el color del icono si es necesario

        // Configura el icono de la p�gina si es necesario
        this.IconImageSource = "registrarauto.png";
        InitializeDatabase();


      
    }


    private byte[] imagenComoBytes;
    private async void Button_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Abrir la galer�a y seleccionar una imagen
            var resultado = await MediaPicker.PickPhotoAsync();

            if (resultado != null)
            {
                // Leer la imagen seleccionada como un arreglo de bytes
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // Esperar a que la tarea se complete y luego obtener el stream
                    var stream = await resultado.OpenReadAsync();
                    // Copiar el stream a la memoria
                    await stream.CopyToAsync(memoryStream);
                    // Guardar la imagen como bytes
                    imagenComoBytes = memoryStream.ToArray();
                }

                // Mostrar la imagen en el controlador Image
                imgFoto.Source = ImageSource.FromStream(() => new MemoryStream(imagenComoBytes));
            }
        }
        catch (Exception ex)
        {
            // Manejar cualquier excepci�n que pueda ocurrir durante el proceso
            await DisplayAlert("Error", $"Se produjo un error: {ex.Message}", "OK");
        }
    }


    listaAutos Actualizarlista = new listaAutos();
    private async void btnGuardar_Clicked(object sender, EventArgs e)
    {

        try
        {
            // Verificar si la placa ya existe en la base de datos
            var existingAuto = await _autosCollection.Find(a => a.Placa == EntryPlacas.Text).FirstOrDefaultAsync();
            if (existingAuto != null)
            {
                await DisplayAlert("Error", "La placa ingresada ya est� registrada.", "OK");
                return;
            }

            var auto = new Auto
            {
                Placa = EntryPlacas.Text,
                Marca = EntryMarca.Text,
                Modelo = EntryModelo.Text,
                Anio = Convert.ToInt32(EntryA�o.Text),
                Color = EntryColor.Text,
                PrecioPorDia = Convert.ToDecimal(EntryPrecioPorDia.Text),
                Foto = imagenComoBytes
            };

            // Verificar si alg�n campo est� vac�o
            if (string.IsNullOrWhiteSpace(auto.Placa) ||
                string.IsNullOrWhiteSpace(auto.Marca) ||
                string.IsNullOrWhiteSpace(auto.Modelo) ||
                string.IsNullOrWhiteSpace(auto.Color) ||
                auto.Foto == null ||
                auto.Anio == 0 ||
                auto.PrecioPorDia == 0)
            {
                await DisplayAlert("Error", "Todos los campos son obligatorios.", "OK");
                return;
            }

            // Insertar el nuevo auto en la base de datos
            await InsertarAuto(auto);
            var mainPage = Application.Current.MainPage as MainPage;
            var crearRentaPage = mainPage?.Children[1] as listaAutos;
            crearRentaPage.CargarAutos();

            // Limpiar los campos despu�s de guardar
            LimpiarCampos();
        }
        catch (FormatException)
        {
            await DisplayAlert("Error", "El a�o o el precio por d�a no tienen un formato v�lido.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Se produjo un error: {ex.Message}", "OK");
        }

    }


    // M�todo para limpiar los campos despu�s de guardar un auto
    public void LimpiarCampos()
    {
        EntryPlacas.Text = string.Empty;
        EntryMarca.Text = string.Empty;
        EntryModelo.Text = string.Empty;
        EntryA�o.Text = string.Empty;
        EntryColor.Text = string.Empty;
        EntryPrecioPorDia.Text = string.Empty;
        imgFoto.Source = null;
        imagenComoBytes = null;
    }

    private async void btnModificar_Clicked(object sender, EventArgs e)
    {
        // Verificar si todos los campos est�n llenos
        if (string.IsNullOrEmpty(EntryPlacas.Text) ||
            string.IsNullOrEmpty(EntryMarca.Text) ||
            string.IsNullOrEmpty(EntryModelo.Text) ||
            string.IsNullOrEmpty(EntryA�o.Text) ||
            string.IsNullOrEmpty(EntryColor.Text) ||
            string.IsNullOrEmpty(EntryPrecioPorDia.Text) ||
            imagenComoBytes == null)
        {
            await DisplayAlert("Error", "Por favor llene todos los campos antes de modificar el auto.", "Aceptar");
            return;
        }

        try
        {
            var auto = new Auto
            {
                Placa = EntryPlacas.Text,
                Marca = EntryMarca.Text,
                Modelo = EntryModelo.Text,
                Anio = Convert.ToInt32(EntryA�o.Text),
                Color = EntryColor.Text,
                PrecioPorDia = Convert.ToDecimal(EntryPrecioPorDia.Text),
                Foto = imagenComoBytes
            };


            // Modificar el auto en la base de datos
            var filter = Builders<Auto>.Filter.Eq(a => a.Placa, auto.Placa);
            var update = Builders<Auto>.Update
                .Set(a => a.Marca, auto.Marca)
                .Set(a => a.Modelo, auto.Modelo)
                .Set(a => a.Anio, auto.Anio)
                .Set(a => a.Color, auto.Color)
                .Set(a => a.PrecioPorDia, auto.PrecioPorDia)
                .Set(a => a.Foto, auto.Foto);

            var updateResult = await _autosCollection.UpdateOneAsync(filter, update);

            if (updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)
            {
                await DisplayAlert("�xito", "Los datos del auto se han modificado correctamente.", "Aceptar");
                var mainPage = Application.Current.MainPage as MainPage;
                var crearRentaPage = mainPage?.Children[1] as listaAutos;
                crearRentaPage.CargarAutos();
                LimpiarCampos();
            }
            else
            {
                await DisplayAlert("Error", "No se pudo modificar los datos del auto.", "Aceptar");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurri� un error al modificar el auto: {ex.Message}", "Aceptar");
        }
    }


    //Codigo Rellenar campos con la placa

    private async void EntryPlacas_Completed(object sender, EventArgs e)
    {
        var entryPlaca = (Entry)sender;
        var placa = entryPlaca.Text;

        try
        {
            // Consultar si la placa existe en la base de datos
            var auto = await _autosCollection.Find(a => a.Placa == placa).FirstOrDefaultAsync();

            if (auto != null)
            {
                // Si el auto existe, rellenar los dem�s campos con los datos correspondientes
                EntryMarca.Text = auto.Marca;
                EntryModelo.Text = auto.Modelo;
                EntryA�o.Text = auto.Anio.ToString();
                EntryColor.Text = auto.Color;
                EntryPrecioPorDia.Text = auto.PrecioPorDia.ToString();

                // Mostrar la imagen correspondiente al auto encontrado
                if (auto.Foto != null && auto.Foto.Length > 0)
                {
                    // Convertir los bytes de la imagen en un ImageSource
                    ImageSource imageSource = ImageSource.FromStream(() => new MemoryStream(auto.Foto));
                    // Asignar la imagen al control de imagen correspondiente en tu interfaz de usuario
                    imgFoto.Source = imageSource;
                    imagenComoBytes=auto.Foto;
                }
                else
                {
                    // Si el auto no tiene imagen, puedes mostrar una imagen predeterminada o dejar el control de imagen vac�o
                    imgFoto.Source = null;
                }
            }
            else
            {
                // Si la placa no existe, puedes mostrar un mensaje o realizar otra acci�n seg�n sea necesario
                await DisplayAlert("Aviso", "La placa ingresada no existe en la base de datos, puede continuar para registrar una nueva placa", "Aceptar");
            }
        }
        catch (Exception ex)
        {
            // Manejar la excepci�n, si ocurre alguna
            Console.WriteLine($"Error al buscar la placa en la base de datos: {ex.Message}");
        }
    }







}