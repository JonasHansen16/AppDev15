<?php
$ANSWERCOUNTQUERY = 
       "SELECT COUNT(answer.id) AS amount 
        FROM client, answer 
        WHERE client.id = ?
        AND client.hash = ?
        AND client.id = answer.clientid 
        AND answer.final = TRUE 
        GROUP BY answer.id 
        ;";
?>
