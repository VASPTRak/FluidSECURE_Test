<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="Fuel_Secure._Default1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="/Content/bootstrap.css" rel="stylesheet" />
    <link href="/Content/homepage.css" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css?family=Open+Sans" rel="stylesheet">
</head>
<body>

    <div id="home-page">
        <div class="jumbotron" style="min-height: 500px">
            <div class="col-sm-12">
                <p style="text-align:right"><a class="btn btn-primary btn-lg" href="/Account/login" role="button">Sign In</a></p>
            </div>
            <div class="container" style="padding-top: 40px;">
                <div class="row">
                    <div class="col-sm-8">
                    <%--    <p><a class="btn btn-primary btn-lg" href="/Account/login" role="button">Sign In</a></p>--%>
                    </div>
                    <div class="col-sm-5">
                    </div>
                    <div class="col-sm-7">
                        <%--  <img class="img-responsive" src="http://mycloud.com/images/home/homepg_detail.png" />--%>
                    </div>
                </div>
            </div>
        </div>
        <div class="container-fluid" style="color: #000;">
            <div class="container">
                <div class="row">
                    <div class="col-md-3 hidden-sm hidden-xs"></div>
                    <div class="col-sm-12 col-xs-12 col-md-6 txtadjust">
                        <p class="h1">FluidSecure</p>
                        <p>
                            FluidSecure is conceptualized as an inexpensive fuel management system designed for low volume single tank and up to 2 hoses maximum.
                        </p>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <hr />
    <footer>
        <p>&copy; <%: DateTime.Now.Year %> - FluidSecure System Software</p>
    </footer>
</body>
</html>
