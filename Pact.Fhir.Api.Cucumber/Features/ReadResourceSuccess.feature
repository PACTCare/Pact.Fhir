Feature: ReadResourceSuccess
	In order to keep track of a patient
	As a doctor
	I want to be able to read his information as a FHIR record

Scenario: Read Patient
	Given I have the patient "John"
	When I create his FHIR record with Prefer "representation"
	Then I should be able to read his record
