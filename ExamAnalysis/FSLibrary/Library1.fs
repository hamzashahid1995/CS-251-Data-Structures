//============================================
// My F# library for exam score analysis.   //
//                                          //
// HAMZA SHAHID                             // 
// U. of Illinois, Chicago                  //
// CS 341, Fall 2017                        //
// Project #04                              //
//============================================
module MyLibrary

#light

//================================================================================
//
// InputScores
//
// Given the complete filepath to a text file of exam scores, 
// inputs the scores and returns them as a list of integers.
//
let InputScores filepath = 
  let L = [ for line in System.IO.File.ReadAllLines(filepath) -> line ]
  List.map (fun score -> System.Int32.Parse(score)) L


//================================================================================
//
// NumScores
//
// Recursively counts the # of scores in the list.
//
let rec NumScores L = 
  match L with
  | []     -> 0
  | hd::tl -> 1 + NumScores tl


//================================================================================
//
// FindMin
//
// Recursively finds the min score in the list.
//
//let rec FindMin L = 
//  -2
//================================================================================
let rec _min minSoFar L = 
  match L with
  | [] -> minSoFar
  | hd::tl -> if hd < minSoFar then
                _min hd tl
              else
                _min minSoFar tl


//================================================================================
let FindMin L = 
  let hd = List.head L
  _min hd (List.tail L)

//================================================================================
//
// FindMax
//
// Recursively finds the max score in the list.
//
//let rec FindMax L = 
//  match L with 
//  | []     -> FindMax
//  | hd::tl -> if hd > _FindMax then 
//


//================================================================================
let rec _max maxSoFar L = 
  match L with
  | [] -> maxSoFar
  | hd::tl -> if hd > maxSoFar then
                _max hd tl
              else
                _max maxSoFar tl


//================================================================================
//
// FindMax
//
// Recursively finds the max score in the list.
//
let FindMax L = 
  let hd = List.head L
  _max hd (List.tail L)


//================================================================================
let rec _sum sumSoFar L = 
  match L with 
   | []      -> sumSoFar
   | hd::tl  -> _sum(sumSoFar + hd) tl 


//================================================================================
let mySum L =
   _sum 0 L
    

//================================================================================
//
// Average
//
// Computes the average of a non-empty list of integers;
// the result is a real number (not an integer).
//
let Average L = 
   match L with 
   | []     -> 0.0
   | hd::tl -> (float(mySum L)) / (float(NumScores L))


//================================================================================
let rec _median L skip isEven =
  match skip with 
  | 0 when isEven -> let first  = List.head L
                     let second = List.head (List.tail L)
                     (float)(first + second) / 2.0
 // | 0 -> List.head L
  | _ -> _median (List.tail L) (skip - 1) isEven


//================================================================================
//
// Median
//
// Computes the median of a non-empty list of integers;
// the result is a real number (not an integer) since the 
// median may be the average of 2 scores if the # of scores
// is even.
//
let Median L = 
  let sort = List.sort L
  let skip = ((List.length sort) - 1) / 2
  let isEven = ((List.length sort) % 2) = 0
  _median sort skip isEven


//================================================================================
let rec map F L = 
  match L with 
  | []     -> []
  | hd::tl -> (F hd) :: (map F tl)
  

//================================================================================
//
// StdDev
//
// Computes the standard deviation of a complete population
// defined by the integer list L.  Returns a real number.
//
let StdDev L = 
   let values = List.map(fun s -> float(s)) L
   let mean =   List.average values
   let diffs =  List.map(fun x -> System.Math.Pow(x - mean,2.0)) values
   let result = System.Math.Sqrt(List.average diffs)
   result


//================================================================================
//
// Histogram
//
// Returns a list containing exactly 5 integers: [A;B;C;D;F].
// The integer A denotes the # of scores in L that fell in the
// range 90-100, inclusive.  B is the # of scores that fell in
// the range 80-89, inclusive.  C is the range 70-79, D is the
// range 60-69, and F is the range 0-59.
//
let Histogram L = 
  let L2 = List.filter (fun x -> x >= 90 && x <= 100) L
  let R = List.length L2

  let L3 = List.filter(fun x -> x >= 80 && x <= 89) L
  let H = List.length L3

  let L4 = List.filter(fun x -> x >= 70 && x <= 79) L
  let K = List.length L4

  let L5 = List.filter(fun x -> x >= 60 && x <= 69) L
  let E = List.length L5
  
  let L6 = List.filter(fun x -> x >= 0 && x <= 59) L
  let J = List.length L6

  [ R; H; K; E; J] 


//================================================================================
//
// Trend
//
// Trend is given 3 lists of integer scores:  L1, L2, L3.  The lists are 
// non-empty, and |L1| = |L2| = |L3|.  L1 are the scores for exam 01, L2
// are the scores for exam 02, and L3 are the scores for exam 03.  The
// lists are in "parallel", which means student i has their scores at 
// position i in each list.  Example: the first exam in each list denote
// the exams for student 0.
//
// Trend returns a new list R such that for each student, R contains a '+'
// if the exam scores were score1 < score2 < score3 --- i.e. the scores
// are trending upward.  R contains a '-' if score1 > score2 > score3, i.e.
// the scores are trending downward.  Otherwise R contains '=' (e.g. if
// score1 < score2 but then score2 > score3).  
//
let rec _Trend L1 L2 L3 acc =
  match L1, L2, L3 with 
  | [], [], [] -> (List.rev acc)
  | [], [], _  -> (List.rev acc) 
  | [], _ , [] -> (List.rev acc)
  | _,  [], [] -> (List.rev acc) 
  | hd1::tl1,hd2::tl2,hd3::tl3 -> if (hd1 < hd2 && hd2 < hd3) then
                                    _Trend tl1 tl2 tl3 ('+'::acc)
                                  else if (hd1 > hd2 && hd2 > hd3) then 
                                    _Trend tl1 tl2 tl3 ('-'::acc)
                                  else 
                                    _Trend tl1 tl2 tl3 ('='::acc)


//================================================================================
let Trend L1 L2 L3 =
    _Trend L1 L2 L3 []


//================================================================================