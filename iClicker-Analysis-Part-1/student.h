#pragma once

using namespace std;

class Student
{
private:
	string ClickerID;
	int StudentsAnswer;
	int CorrectAnswer;

public:
	// default constructor
	Student(std::string clickerID, int quesAns, int ansCorrec)
		: ClickerID(clickerID), StudentsAnswer(quesAns), CorrectAnswer(ansCorrec)
	{
	}
	string getClickerID() const
	{
		return ClickerID;
	}
	int answered() const
	{
		return StudentsAnswer;
	}
	int correct() const
	{
		return CorrectAnswer;
	}
	void incrimentQuestionsAns()
	{
		StudentsAnswer++;
	}
	void incrimentCorrectAns()
	{
		CorrectAnswer++;
	}
};

