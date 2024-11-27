<?php
// Assuming you've established a database connection
$conn = mysqli_connect("localhost", "root", "", "bookstore");

session_start();

if ($_SERVER["REQUEST_METHOD"] == "POST") {
    // Check if the form is submitted for login
    if (isset($_POST["login-email"]) && isset($_POST["login-password"])) {
        $username = $_POST["login-email"];
        $password = $_POST["login-password"];

        $sql = "SELECT * FROM users WHERE email = '$username'";
        $result = mysqli_query($conn, $sql);

        if ($result) {
            $row = mysqli_fetch_assoc($result);
            if ($row && password_verify($password, $row["password"])) {
                // Set the session variable upon successful login
                $_SESSION['user_id'] = $row['id'];
                header("Location: cart.php");
                exit();
            } else {
                echo "Invalid email or password";
            }
        } else {
            echo "Invalid email or password";
        }
    }

    // Check if the form is submitted for registration
    if (isset($_POST["register-email"]) && isset($_POST["register-password"]) && isset($_POST["confirm-password"])) {
        $regUsername = $_POST["register-email"];
        $regPassword = password_hash($_POST["register-password"], PASSWORD_DEFAULT);
        $regEmail = $_POST["register-email"];

        $sql = "INSERT INTO users (username, password, email) VALUES ('$regUsername', '$regPassword', '$regEmail')";

        if (mysqli_query($conn, $sql)) {
            echo "Registration successful!";
        } else {
            echo "Error: " . $sql . "<br>" . mysqli_error($conn);
        }
    }
}

mysqli_close($conn);
?>

<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Login/Register - TextBookTreasures</title>
    <link rel="stylesheet" href="styles.css">
</head>

<body class="login-register">
    <header>
        <img src="images/Logo.png" alt="TextBookTreasures Logo" id="logo">
        <h1>TextBookTreasures</h1>
        <div class="header-buttons">
            
            
            <a href="HomePage.php"><button >Homepage</button></a>
        </div>
    </header>

    <main>
        <div class="login-register-container">
            <!-- Login Form -->
            <div class="login-form">
                <h2>Login</h2>
                <form action="#" method="post">
                    <label for="login-email">Email:</label>
                    <input type="email" id="login-email" name="login-email" required>
                    <label for="login-password">Password:</label>
                    <input type="password" id="login-password" name="login-password" required>
                    <button type="submit">Login</button>
                </form>
            </div>

            <!-- Register Form -->
            <div class="register-form">
                <h2>Register</h2>
                <form action="#" method="post">
                    <label for="register-email">Email:</label>
                    <input type="email" id="register-email" name="register-email" required>
                    <label for="register-password">Password:</label>
                    <input type="password" id="register-password" name="register-password" required>
                    <label for="confirm-password">Confirm Password:</label>
                    <input type="password" id="confirm-password" name="confirm-password" required>
                    <button type="submit">Register</button>
                </form>
            </div>
        </div>
    </main>

    <footer>
        <!-- Footer content if needed -->
        <img src="images/Bottom.png" alt="Bottom Image" class="full-width-image">
        <div class="newsletter-section">
            <p>Subscribe to our newsletter for updates</p>
            <div class="newsletter-form">
                <label for="email">Email:</label>
                <input type="email" id="email" name="email" placeholder="Your email...">
                <button>Subscribe</button>
            </div>
        </div>
    </footer>
</body>

</html>
