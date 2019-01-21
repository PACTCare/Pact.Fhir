Feature: CreateResourceSuccess
	In order to keep track of a patient
	As a doctor
	I want to be able to create his information as a FHIR record

Scenario: Create Patient
	Given I have the patient "Max"
	When I create his FHIR record with Prefer "representation"
	Then I should see a valid patient JSON representation
