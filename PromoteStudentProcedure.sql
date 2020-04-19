use university;

alter procedure PromoteStudent @studyName nvarchar(50),
@semester int
as 
begin
	Set XACT_abort on;
	Begin Tran
	
	Declare @idStudy int = (select IdStudy from Studies where Name=@studyName);
	Declare @sem int = (select semester from Enrollment where Semester=@semester);
	if (@idStudy is null or @sem is null)
	begin
		raiserror('Study or semester does not exists', 20, -1) with log;
	end

	Declare @nextEnrollmentId int = (select IdEnrollment from Enrollment
										where IdStudy=@idStudy and semester=@semester+1);

	IF @nextEnrollmentId Is null
	begin
		select @nextEnrollmentId=max(IdEnrollment)+1  
		from Enrollment;

		Insert into Enrollment 
		values(@nextEnrollmentId,@semester+1,@idStudy,GetDate());
		
	end;

	Declare @currentEnrollmentId int = (select IdEnrollment from Enrollment
										where IdStudy=@idStudy and semester=@semester);

	update Student
	set IdEnrollment=@nextEnrollmentId
	where IdEnrollment=@currentEnrollmentId;

	commit

	select * from Enrollment where IdEnrollment=@nextEnrollmentId; 
	
end;

exec PromoteStudent 'IT',2;

select *from Student

select * from Enrollment

update student
set idenrollment=1
