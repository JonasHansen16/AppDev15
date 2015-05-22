<?php
$INTROQUERY = 
       "SELECT questionnaire.intro, questionnaire.title 
        FROM client, form, questionnaire 
        WHERE client.id = ? 
        AND client.hash = ? 
        AND client.formid = form.id 
        AND form.airid = questionnaire.id 
        ;";
?>
