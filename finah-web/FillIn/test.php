<?php
/// TEST WHETHER OR NOT YOUR MYSQLI INSTALLATION WAS SUCCESSFUL \\\

if (!function_exists('mysqli_init') && !extension_loaded('mysqli')) {
    echo 'mysqli has abandoned you. Panic!';
} else {
    echo 'mysqli has decided to grace you with its presence...for now.';
}
echo '<br/>';
echo '<br/>';
echo 'You\'re probably panicking right now. Here are a few bits of information to help you summon our Lord and Saviour mysqli:';
echo '<br/>';
echo '1) The php sapi name: ';
echo php_sapi_name();
echo ' - Use this to determine which config file PHP uses by default.';
echo '<br/>';
echo '2) The full absolute path of the config file that was loaded: ';
echo php_ini_loaded_file();
echo ' - If this value is blank, no configuration file is being loaded. To resolve this, create php.ini at the configuration file path in the table below. If all else fails, place it in %ROOT%\windows .';
echo '<br/>';
echo '3) Remember that php configuration stuff usually requires a full restart of the PHP engine (read: reboot computer) in order to take effect.';
echo '<br/>';
echo '4) Further information, useful fields are configuration file path, loaded configuration file and everything under the header mysqli.';
echo phpinfo();
?>