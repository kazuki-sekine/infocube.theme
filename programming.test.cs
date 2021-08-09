#nullable enable 
 
using System.Collections.Generic; 
using System.Linq; 
using System.Text.Json; 
using System.Text.Json.Serialization; 
using System.Text.RegularExpressions; 
 
/// ログ１エントリのデータ型 
public record LogEntry { 
  [JsonPropertyName("time")]   
  public long   Time  { get; init; } 
 
  [JsonPropertyName("score")]   
  public double Score { get; init; } 
} 
 
/// １ユーザのデータ型 
public record User { 
  [JsonPropertyName("id")] 
  public int              Id   { get; init; } 
 
  [JsonPropertyName("name")] 
  public string?          Name { get; init; } 
 
  [JsonPropertyName("logs")]   
  public IList<LogEntry>? Log  { get; init; } 
} 
 
/// JSON 文字列をパースして、設問の答えを出力するクラス 
public static class DataAnalyzer { 
  // ito    は sato を time += 1000, score -= 10 した追加サンプルデータ 
  // tanaka は sato を time += 2000, score -= 20 した追加サンプルデータ 
  public static string JsonString = @"users = [ 
  { 
    ""id"": 1, 
    ""name"": ""sato"", 
    ""logs"": [ 
      {""time"":1553101195196, ""score"":84}, 
      {""time"":1552993677688, ""score"":43}, 
      {""time"":1552901583366, ""score"":62}, 
      {""time"":1552781080350, ""score"":95}, 
      {""time"":1552689530078, ""score"":51} 
    ] 
  }, 
  { 
    ""id"": 2, 
    ""name"": ""ito"", 
    ""logs"": [ 
      {""time"":1553101196196, ""score"":74}, 
      {""time"":1552993678688, ""score"":33}, 
      {""time"":1552901584366, ""score"":52}, 
      {""time"":1552781081350, ""score"":85}, 
      {""time"":1552689531078, ""score"":41} 
    ] 
  }, 
  { 
    ""id"":3, 
    ""name"": ""tanaka"", 
    ""logs"": [ 
      {""time"":1553101197196, ""score"":64}, 
      {""time"":1552993679688, ""score"":23}, 
      {""time"":1552901585366, ""score"":42}, 
      {""time"":1552781082350, ""score"":75}, 
      {""time"":1552689532078, ""score"":31} 
    ] 
  } 
]"; 
  public static IList<User>? JsonObject = 
    // 文字列先頭の "users = " 部分を捨てて、JSON を User データ型のリストにパースする 
    JsonSerializer.Deserialize<IList<User>>(Regex.Replace(JsonString, @"^\s*users\s*=\s*", "", RegexOptions.IgnoreCase)); 
 
  // 注意 ： 集計関数 .Max(), .Average() を使う場合、本来はその直前に .DefaultIfEmpty() を入れて、ヒットしなかった場合に備える 
  public static void Question1(int n) => 
    JsonObject? 
      // data extraction 
      .Select( user => new { User = user, MaxScore = user.Log?.Max( log => log.Score ) ?? 0d } ) // User データと最大スコアを組にした新しいデータを返す 
      .OrderByDescending( result => result.MaxScore )                                            // 最大スコアで降順ソートする 
      .Take(n)                                                                                   // 最初の n 個を取る 
      // display 
      .Select( (result, i) => { 
        Console.WriteLine($"Q1. # {i + 1} ======== (n = {n})"); 
        Console.WriteLine($"  Id       = {result.User.Id}"); 
        Console.WriteLine($"  Name     = {result.User.Name}"); 
        Console.WriteLine($"  MaxScore = {result.MaxScore}"); 
        return true; 
      } ).ToList(); 
 
  public static void Question2(int n) => 
    JsonObject? 
      // data extraction 
      .Select( user => new { User = user, MaxTime = user.Log?.Max( log => log.Time ) ?? 0d } ) // User データと最大時刻を組にした新しいデータを返す 
      .OrderByDescending( result => result.MaxTime )                                           // 最大時刻で降順ソートする 
      .Take(n)                                                                                 // 最初の n 個を取る 
      // display 
      .Select( (result, i) => { 
        Console.WriteLine($"Q2. # {i + 1} ======== (n = {n})"); 
        Console.WriteLine($"  Id      = {result.User.Id}"); 
        Console.WriteLine($"  Name    = {result.User.Name}"); 
        Console.WriteLine($"  MaxTime = {result.MaxTime}"); 
        return true; 
      } ).ToList(); 
 
  public static void Question3A(long floor, long ceil) =>　// 各 User の指定時間内ログに対する各 User ごとの平均スコア 
    JsonObject? 
      // data extraction 
      .Select( user => new { // User データ、と、指定時刻 floor と ceil に挟まれる条件で抽出したログの平均スコア、を組にした新しいデータを返す 
        User         = user, 
        AverageScore = user.Log?.Where( log => floor <= log.Time && log.Time <= ceil ).Average( log => log.Score ) ?? 0d, 
      } ) 
      // display 
      .Select( (result, i) => { 
        Console.WriteLine($"Q3A. # {i + 1} ======== ({floor} <= Time <= {ceil})"); 
        Console.WriteLine($"  Id           = {result.User.Id}"); 
        Console.WriteLine($"  Name         = {result.User.Name}"); 
        Console.WriteLine($"  AverageScore = {result.AverageScore}"); 
        return true; 
      } ).ToList(); 
 
  public static void Question3B(long floor, long ceil) { // 全 User の指定時間内全ログに対する単一の平均スコア 
    var average = 
      JsonObject? 
        // data extraction 
        .SelectMany( user => user.Log ?? Enumerable.Empty<LogEntry>() ) // 各 User のログを連結したリストを返す 
        .Where( log => floor <= log.Time && log.Time <= ceil )          // 指定時刻 floor と ceil に挟まれる条件で抽出する 
        .Average( log => log.Score ) ?? 0d;                             // スコアの平均を算出する 
 
      // display 
      Console.WriteLine($"Q3B. ======== ({floor} <= Time <= {ceil})"); 
      Console.WriteLine($"  TotalAverageScore = {average}"); 
  } 
} 
 
// 実行 
DataAnalyzer.Question1(5); 
DataAnalyzer.Question2(5); 
DataAnalyzer.Question3A(1552993677688, 1553101195196); 
DataAnalyzer.Question3B(1552993677688, 1553101195196);
