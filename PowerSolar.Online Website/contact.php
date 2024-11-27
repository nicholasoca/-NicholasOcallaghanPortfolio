<?php
if ($_SERVER["REQUEST_METHOD"] == "POST") {
  // Get form data
  $name = $_POST["name"];
  $email = $_POST["email"];
  $message = $_POST["message"];

  // Compose email
  $to = "info@powersolar.online"; 
  $subject = "New contact form submission";
  $body = "Name: $name\n";
  $body .= "Email: $email\n";
  $body .= "Message: $message\n";

  // Send email
  $success = mail($to, $subject, $body);

  // Redirect to thank you page
  if ($success) {
    header("Location: ThankYou.html"); 
    exit;
  } else {
    echo "Oops! Something went wrong.";
  }
}
?>