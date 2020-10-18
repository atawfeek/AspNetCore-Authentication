# AspNetCore-Authentication

https://code-maze.com/authentication-aspnetcore-jwt-1/

https://code-maze.com/using-refresh-tokens-in-asp-net-core-authentication/

The Big Picture
Before we get into the implementation of authentication and authorization, let’s have a quick look at the big picture. There is an application that has a login form. A user enters its username, password and presses the login button. After pressing the login button, a client (eg web browser) sends the user’s data to the server’s API endpoint.web authentication big pictureWhen the server validates the user’s credentials and confirms that the user is valid, it’s going to send an encoded JWT to the client. JSON web token is basically a JavaScript object that can contain some attributes of the logged-in user. It can contain a username, user subject, user roles or some other useful information.

At the client-side, we store the JWT in the browser’s local storage to remember the user’s login session. We may also use the information from the JWT to enhance the security of our application as well.

Introduction to JSON Web Tokens
JSON web tokens enable a secure way to transmit data between two parties in the form of a JSON object. It’s an open standard and it’s a popular mechanism for web authentication. In our case, we are going to use JSON web tokens to securely transfer a user’s data between the client and the server.

JSON web tokens consist of three basic parts: the header, payload, and the signature.

One real example of JSON web token:

JWT example

Every part of all three parts is shown in a different color:

Header
The first part of JWT is the Header, which is a JSON object encoded in the base64 format. The header is a standard part of JWT and we don’t have to worry about it. It contains information like the type of token and the name of the algorithm.

Header Sample:
{
  "alg": "HS256",
  "typ": "JWT"
}
Payload
After the Header, we have a Payload which is also a JavaScript object encoded in the base64 format. The payload contains some attributes about the logged-in user. For example, it can contain user id, user subject, and the information about whether a user is an admin user or not. JSON web tokens are not encrypted and can be decoded with any base64 decoder so please never include sensitive information in the Payload.

Payload sample:
{
  "sub": "1234567890",
  "name": "John Doe",
  "iat": 1516239022
}
Signature
Finally, we have the Signature part. Usually, the server uses the signature part to verify whether the token contains valid information, the information which the server is issuing. It is a digital signature that gets generated by combining the header and the payload together. Moreover, it’s based on a secret key that only the server knows.

JWT signature composition

So, if malicious users try to modify the values in the payload, they have to recreate the signature and for that purpose, they need the secret key which the only server has. At the server-side, we can easily verify if the values are original or not by comparing the original signature with a new signature computed from the values coming from the client.

So, we can easily verify the integrity of our data just by comparing the digital signatures. This is the reason why we use JWT.

Creating ASP.NET Core Web Service
Now let’s create a brand new ASP.NET Core project. We can create a new Web API project with .NET Core CLI or you can use Visual Studio 2019. We are going to use the CLI for this demo so if you don’t want to install Visual Studio 2019, you don’t have to. Once we create the project you can open it with any code editor like Visual Studio Code, Notepad++, WebStorm or even Visual Studio.

To create a new Web API project with .NET Core CLI we need to:

Open a command-line terminal (cmd)
Navigate the terminal window to the directory where we want to create the project
Run the command:
dotnet new webapi -n webapplication
As a result, we are going to have a new .NET Core Web API project template in the current directory with some boilerplate code.
We can open the launchSettings.json file and modify it by removing the https://localhost:5001 part from the applicationUrl property.

Additionally, let’s navigate the terminal window to the project’s root directory.

We need to navigate to our project:
cd webapplication
And run the application locally:
dotnet run
As a result, this is going to spin up a web server locally and it will host our application at http://localhost:5000. If we type the same address in the browser’s address bar, you are going to get no response as there is no resource available at that endpoint. Now let’s type http://localhost:5000/weatherforecast to get some result:
Http GET Response

Awesome! So far so good.

In the next step, we are going to configure JWT authentication in our application.

Configuring JWT Authentication
To configure JWT authentication in .NET Core, we need to modify Startup.csfile. It’s a bootstrapper class that runs when our application starts. Inside the file, we have the ConfigureSerivces method that adds services to the IServiceCollection container, thus making them available for the constructor injection.

JWT’s support is built into ASP.NET Core 3.0 and we are going to configure an authentication middleware for JSON web tokens.

For the sake of simplicity, we are going to add all the code inside the ConfigureServices method. But the better practice is to use Extension methods so we could free our ConfigureServices method from extra code lines. If you want to learn how to do that, and to learn more about configuring the .NET Core Web API project, check out: .NET Core Service Configuration.

We need to modify the ConfigureServices method to add the JWT support:
public void ConfigureServices(IServiceCollection services)
{
    services.AddAuthentication(opt => {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "http://localhost:5000",
            ValidAudience = "http://localhost:5000",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"))
        };
    });
    services.AddControllers();
}
We have to install the Microsoft.AspNetCore.Authentication.JwtBearer library in order for this to work.
Code Explanation
Firstly, we register the JWT authentication middleware by calling the method AddAuthentication on the ISerivceCollectioninterface. Next, we specify the authentication scheme JwtBearerDefaults.AuthenticationScheme as well as ChallengeScheme. We also provide some parameters that will be used while validating JWT.
Excellent.

We’ve successfully configured the JWT authentication.

According to the configuration, the token is going to be valid if:

The issuer is the actual server that created the token (ValidateIssuer=true)
The receiver of the token is a valid recipient (ValidateAudience=true)
The token has not expired (ValidateLifetime=true)
The signing key is valid and is trusted by the server (ValidateIssuerSigningKey=true)
Additionally, we are providing values for the issuer, audience, and the secret key that the server uses to generate the signature for JWT. We are going to hardcode both username and password for the sake of simplicity. But, the best practice is to put the credentials in a database or a configuration file or to store the secret key into the environment variable.

We need to do one more step to make our authentication middleware available to the application.

Add the app.UseAuthentication() in the Configure method:
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}
And that’s all we need to configure the JWT authentication in ASP.NET Core.
Securing API Endpoints
We already have an API endpoint /weatherforecast to get some example weather information and that endpoint is not secure. Anyone can send a request to http://localhost:5000/weatherforecast fetch the values. So, in this section, we are going to add a new api/customers endpoint to serve a list of the customers.  This endpoint is going to be secure from anonymous users and only logged-in users can consume it.

Now let’s add an empty CustomersController in the Controllers folders. Inside the controller, we are going to add a Get action method that is going to return an array of customers. More importantly, we are going to add an extra security layer by decorating the action method with the[Authorize] attribute so only logged-in users can access the route.

Let’s modify the CustomersController class:
[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    // GET api/values
    [HttpGet,Authorize]
    public IEnumerable<string> Get()
    {
     return new string[] { "John Doe", "Jane Doe" };
    }
}
Authorize attribute on top of the GET method restricts access to only authorized users. Only users who are logged-in can access the list of customers. Therefore, this time if you make a request to http://localhost:5000/api/customers from the browser’s address bar, instead of getting a list of customers, you are going to get a 401 Not Authorized response.
HTTP 401 Not Authorized response

Adding Login Endpoint
To authenticate anonymous users, we have to provide a login endpoint so the users can log-in and access protected resources. A user is going to provide a username, password and if the credentials are valid we are going to issue a JSON web token for the requesting client.

In addition, before we start implementing the authentication controller we need to add a LoginModel to hold user’s credentials on the server. LoginModel is a simple class that contains two properties: UserName and Password.  We are going to create a Models folder in the root directory and inside it a LoginModel class:
public class LoginModel
{
    public string UserName { get; set; }
    public string Password { get; set; }
}
Now let’s create the AuthController inside the Controllers folder. Inside the AuthControllerwe are going to validate the user’s credentials. If the credentials are valid, we are going to issue a JSON web token. For this demo, we are going to hardcode the username and password to implement a fake user. After validating the user’s credentials we are going to generate a JWT with a secret key. JWT uses the secret key to generate the signature.
Let’s implement the AuthController:
[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    // GET api/values
    [HttpPost, Route("login")]
    public IActionResult Login([FromBody]LoginModel user)
    {
        if (user == null)
        {
            return BadRequest("Invalid client request");
        }
        if (user.UserName == "johndoe" && user.Password == "def@123")
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokeOptions = new JwtSecurityToken(
                issuer: "http://localhost:5000",
                audience: "http://localhost:5000",
                claims: new List<Claim>(),
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: signinCredentials
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return Ok(new { Token = tokenString });
        }
        else
        {
            return Unauthorized();
        }
    }
}
Code Explanation
First of all, notice the use of the [HttpPost] attribute. After applying this attribute to action methods as a result the API endpoint only responds to HTTP POST requests. Inside the login method, we are creating the SymmetricSecretKey with the secret key value superSecretKey@345. Then, we are creating the objec SigningCredentials and as arguments, we provide a secret key and the name of the algorithm that we are going to use to encode the token.

Here comes the interesting part.

The first two steps are the standard steps that you don’t need to worry about. The third step is the one that we are interested in. In the third step we are creating the JwtSecurityToken object with some important parameters:

Issuer: The first parameter is a simple string representing the name of the webserver that issues the token
Audience: The second parameter is a string value representing valid recipients
Claims: The third argument is a list of user roles, for example, the user can be an admin, manager or author (we are going to add roles in the next post)
Expires: The fifth argument is DateTime object that represents the date and time after which the token expires
Finally, we create a string representation of JWT by calling the WriteToken method on JwtSecurityTokenHandler. Finally, we are returning JWT in a response. As a response, we have created an anonymous object that contains only the Token property.

Testing the Login API
So, let’s save all the files, navigate to the root directory of the project, and run the application with the .NET Core CLI command:
dotnet run
Let’s launch any web proxy tool that is capable of composing and sending web requests over HTTP. (You can use your preferred REST client). We are going to use the Postman.
In the request settings options, we are going to enter the URL: http://localhost:5000/api/auth/login, choose POST request option from the right side and paste  in the payload the JSON data:
{
   "UserName":"johndoe",
   "Password": "def@123"
} 
And for headers, we have to select Content-Type for the key and application/json for the value.
Authentication POST request for the JWT
And press the send request button.

In the response section, we are going to see a 200 OK response with the JWT string in the response body:Postam Jwt Response
