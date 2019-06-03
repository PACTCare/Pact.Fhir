# PACT Fhir
[![Build status](https://florencechat.visualstudio.com/PACT%20Fhir/_apis/build/status/PACT%20Fhir-CI)](https://florencechat.visualstudio.com/PACT%20Fhir/_build/latest?definitionId=13)


> FHIR® – Fast Healthcare Interoperability Resources (hl7.org/fhir) – is a next generation standards framework created by HL7.

FHIR solutions are built from a set of modular components called "Resources". These resources can easily be assembled into working systems that solve real world clinical and administrative problems at a fraction of the price of existing alternatives. FHIR is suitable for use in a wide variety of contexts – mobile phone apps, cloud communications, EHR-based data sharing, server communication in large institutional healthcare providers, and much more.

<a href="https://www.youtube.com/watch?v=BXnkzEUQsTY&t="><img src="https://i.imgur.com/s2ced6T.png" width="500px" alt="FHIR PoC"></a>

---

## Table of Contents

- [Security](#security)
- [Background](#background)
- [Install](#install)
- [Contributing](#contributing)
  - [Donate](#donate)
  - [Code](#code)
- [License](#license)

## Security

The IOTA part of this project is using the MAM protocol to encrypt and read data. 
See: https://blog.iota.org/introducing-masked-authenticated-messaging-e55c1822d50e

## Background

This project is part of the "Untangle Care" ecosystem. The goal of the “Untangle Care” project is to develop key IOTA-based tools that will allow healthcare data to flow between healthcare institution-based solutions (e.g. Electronic Medical Records) and citizen-based solutions (e.g. Personal Health Records) in a secure, seamless and healthcare-interoperable way (using Fast Healthcare Interoperable Resources - FHIR).

For more information visit the [IOTA transparency](https://transparency.iota.org/) site and search for Untangle Care or have a look at the [ecosystem site](https://ecosystem.iota.org/projects/untangle-care).


## Install

> Developing with your own data source

Grab the package from nuget (https://www.nuget.org/packages/Pact.Fhir.Core). To start developing you can either use the IOTA implementation or create your own, by implementing the FhirRepository.


> Developing with the IOTA implementation

Grab the package from nuget (https://www.nuget.org/packages/Pact.Fhir.Iota). If you are familiar with IOTA you should cope with the code immediately.

> Documentation

https://florencedigitalhealth.atlassian.net/wiki/spaces/IF/overview

## Contributing

### Donate
```
LRKZARMRXNBSUAOGYOMCRBAZAK9ZSVWTIXOVFTGTX9FRZVUNIM9NLBEZPPZIDD9MQHVCOFNELKGVCIYVWQGRWYZU9X
```

### Code
If you want to help either join our **[discord server](https://discord.gg/VMj7PFN)** or you can open issues for bugs you've found or features you think are missing. You can also submit pull requests to this repository.

If editing the README, please conform to the [standard-readme specification](https://github.com/RichardLitt/standard-readme).

## License

[MIT License](https://github.com/PACTCare/Pact.Fhir/blob/master/LICENSE) © PACT Care B.V.