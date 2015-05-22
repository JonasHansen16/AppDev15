<?php
$EXISTS = "http://nah-back-15.azurewebsites.net/api/client/Exists/";
$START = "http://nah-back-15.azurewebsites.net/api/client/Start/";
$DONE = "http://nah-back-15.azurewebsites.net/api/client/Done/"; 
$NEXT = "http://nah-back-15.azurewebsites.net/api/question/Next/"; 
$INTRO = "http://nah-back-15.azurewebsites.net/api/questionnaire/Intro/";
$SETSTART = "http://nah-back-15.azurewebsites.net/api/client/SetStart/";
$COUNT = "http://nah-back-15.azurewebsites.net/api/questionnaire/Count/";
$IMAGE = "http://nah-back-15.azurewebsites.net/api/question/Image/";

function rest_helper($url, $params = null, $verb = 'POST', $format = 'json')
{
  $cparams = array(
    'http' => array(
      'method' => $verb,
      'ignore_errors' => false
    )
  );
  if ($params !== null) {
    $params = http_build_query($params);
    $cparams['http']['content'] = $params;
    $url .= '?' . $params;
  }

  $context = stream_context_create($cparams);
  $fp = fopen($url, 'rb', false, $context);
  if (!$fp) {
    $res = false;
  } else {
    $res = stream_get_contents($fp);
  }

  if ($res === false) {
    throw new Exception("$verb $url failed: $php_errormsg");
  }

  switch ($format) {
    case 'json':
      $r = json_decode($res);
      if ($r === null) {
        throw new Exception("failed to decode $res as json");
      }
      return $r;

    case 'xml':
      $r = simplexml_load_string($res);
      if ($r === null) {
        throw new Exception("failed to decode $res as xml");
      }
      return $r;
  }
  return $res;
}

class question
{
    public $id;
    public $title;
    public $text;
}

class introduction
{
    public $title;
    public $text;
}

function base64_to_jpeg($base64_string, $output_file) {
    $ifp = fopen($output_file, "wb"); 

    $data = explode(',', $base64_string);

    fwrite($ifp, base64_decode($data[1])); 
    fclose($ifp); 

    return $output_file; 
}

error_reporting(E_ALL ^ E_NOTICE);

$result = rest_helper($EXISTS, array('id' => 1, 'hash' => "0e0d33ef89ba56cfdd54"));
var_dump($result);

$result = rest_helper($START, array('id' => 1, 'hash' => "0e0d33ef89ba56cfdd54"));
var_dump($result);

$result = rest_helper($DONE, array('id' => 1, 'hash' => "0e0d33ef89ba56cfdd54"));
var_dump($result);

$result = rest_helper($NEXT, array('id' => 1, 'hash' => "0e0d33ef89ba56cfdd54"));
$q = new question();
$q->id = $result->Id;
$q->title = $result->Title;
$q->text = $result->Text;
var_dump($q);

$result = rest_helper($INTRO, array('id' => 1, 'hash' => "0e0d33ef89ba56cfdd54"));
$i = new introduction();
$i->text = $result->Text;
$i->title = $result->Title;
var_dump($i);

$result = rest_helper($SETSTART, array('id' => 1, 'hash' => "0e0d33ef89ba56cfdd54"));
var_dump($result);

$result = rest_helper($COUNT, array('id' => 1, 'hash' => "0e0d33ef89ba56cfdd54"));
var_dump($result);

$image = rest_helper($IMAGE, array('id' => 1, 'hash' => "0e0d33ef89ba56cfdd54", 'qid' => 1));
var_dump($image);
//$image = base64_decode($image);
//var_dump($image);
?>

<img class="questionimage" src="data:image/png;base64,<?php echo($image);?>"></img>