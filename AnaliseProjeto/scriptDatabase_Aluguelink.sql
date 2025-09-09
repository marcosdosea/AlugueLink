-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema aluguelink
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `aluguelink` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci ;
USE `aluguelink` ;

-- -----------------------------------------------------
-- Table `aluguelink`.`locador`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aluguelink`.`locador` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `nome` VARCHAR(100) NOT NULL,
  `email` VARCHAR(100) NOT NULL,
  `telefone` VARCHAR(20) NOT NULL,
  `cpf` CHAR(11) NOT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_unicode_ci;

-- -----------------------------------------------------
-- Table `aluguelink`.`imovel`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aluguelink`.`imovel` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `idLocador` INT NOT NULL,
  `cep` VARCHAR(8) NOT NULL,
  `logradouro` VARCHAR(100) NOT NULL,
  `numero` VARCHAR(5) NOT NULL,
  `complemento` VARCHAR(50) NULL DEFAULT NULL,
  `bairro` VARCHAR(50) NOT NULL,
  `cidade` VARCHAR(45) NOT NULL,
  `estado` CHAR(2) NOT NULL,
  `tipo` ENUM('C', 'A', 'PC') NOT NULL,
  `quartos` INT NOT NULL,
  `banheiros` INT NOT NULL,
  `area` DECIMAL(10,2) NOT NULL,
  `vagasGaragem` INT NOT NULL,
  `valor` DECIMAL(10,2) NOT NULL,
  `descricao` VARCHAR(200) NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_imovel_locador_idx` (`idLocador` ASC) VISIBLE,
  CONSTRAINT `fk_imovel_locador`
    FOREIGN KEY (`idLocador`)
    REFERENCES `aluguelink`.`locador` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_unicode_ci;

-- -----------------------------------------------------
-- Table `aluguelink`.`locatario`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aluguelink`.`locatario` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `nome` VARCHAR(100) NOT NULL,
  `email` VARCHAR(100) NOT NULL,
  `telefone1` CHAR(11) NOT NULL,
  `telefone2` CHAR(11) NOT NULL,
  `cpf` CHAR(11) NOT NULL,
  `cep` VARCHAR(8) NOT NULL,
  `logradouro` VARCHAR(100) NOT NULL,
  `numero` VARCHAR(5) NOT NULL,
  `complemento` VARCHAR(50) NULL DEFAULT NULL,
  `bairro` VARCHAR(50) NOT NULL,
  `cidade` VARCHAR(45) NOT NULL,
  `estado` CHAR(2) NOT NULL,
  `profissao` VARCHAR(100) NULL DEFAULT NULL,
  `renda` DECIMAL(10,2) NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `cpf_UNIQUE` (`cpf` ASC) VISIBLE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_unicode_ci;

-- -----------------------------------------------------
-- Table `aluguelink`.`aluguel`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aluguelink`.`aluguel` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `dataInicio` DATE NOT NULL,
  `dataFim` DATE NOT NULL,
  `status` ENUM('A', 'F', 'P') NULL DEFAULT NULL,
  `dataAssinatura` DATE NOT NULL,
  `idlocatario` INT NOT NULL,
  `idimovel` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_aluguel_locatario1_idx` (`idlocatario` ASC) VISIBLE,
  INDEX `fk_aluguel_imovel1_idx` (`idimovel` ASC) VISIBLE,
  CONSTRAINT `fk_aluguel_locatario1`
    FOREIGN KEY (`idlocatario`)
    REFERENCES `aluguelink`.`locatario` (`id`)
    ON DELETE RESTRICT
    ON UPDATE RESTRICT,
  CONSTRAINT `fk_aluguel_imovel1`
    FOREIGN KEY (`idimovel`)
    REFERENCES `aluguelink`.`imovel` (`id`)
    ON DELETE RESTRICT
    ON UPDATE RESTRICT)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_unicode_ci;

-- -----------------------------------------------------
-- Table `aluguelink`.`pagamento`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aluguelink`.`pagamento` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `valor` DECIMAL(10,2) NOT NULL,
  `dataPagamento` DATETIME NOT NULL,
  `tipoPagamento` ENUM('CD', 'CC', 'P', 'B') NOT NULL,
  `idaluguel` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_pagamento_aluguel1_idx` (`idaluguel` ASC) VISIBLE,
  CONSTRAINT `fk_pagamento_aluguel1`
    FOREIGN KEY (`idaluguel`)
    REFERENCES `aluguelink`.`aluguel` (`id`)
    ON DELETE RESTRICT
    ON UPDATE RESTRICT)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_unicode_ci;

-- -----------------------------------------------------
-- Table `aluguelink`.`manutencao`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aluguelink`.`manutencao` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `descricao` VARCHAR(200) NOT NULL,
  `dataSolicitacao` DATETIME NOT NULL,
  `status` ENUM('P', 'A', 'C') NOT NULL COMMENT 'P - PEDIDO REALIZADO\nA - ATENDIDA\nC - CANCELADA',
  `valor` DECIMAL(10,2) NOT NULL,
  `idimovel` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_manutencao_imovel1_idx` (`idimovel` ASC) VISIBLE,
  CONSTRAINT `fk_manutencao_imovel1`
    FOREIGN KEY (`idimovel`)
    REFERENCES `aluguelink`.`imovel` (`id`)
    ON DELETE RESTRICT
    ON UPDATE RESTRICT)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_unicode_ci;

-- -----------------------------------------------------
-- Table `aluguelink`.`locatariolocador`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aluguelink`.`locatariolocador` (
  `idlocatario` INT NOT NULL,
  `idlocador` INT NOT NULL,
  PRIMARY KEY (`idlocatario`, `idlocador`),
  INDEX `fk_locatariolocador_locador1_idx` (`idlocador` ASC) VISIBLE,
  INDEX `fk_locatariolocador_locatario1_idx` (`idlocatario` ASC) VISIBLE,
  CONSTRAINT `fk_locatariolocador_locatario1`
    FOREIGN KEY (`idlocatario`)
    REFERENCES `aluguelink`.`locatario` (`id`)
    ON DELETE RESTRICT
    ON UPDATE RESTRICT,
  CONSTRAINT `fk_locatariolocador_locador1`
    FOREIGN KEY (`idlocador`)
    REFERENCES `aluguelink`.`locador` (`id`)
    ON DELETE RESTRICT
    ON UPDATE RESTRICT)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_unicode_ci;

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;

-- =====================================================
-- Bloco adicional: Criação do banco de Identity (IdentityUsers)
-- =====================================================

-- Garantir charset padrão
SET NAMES utf8mb4;
SET CHARACTER SET utf8mb4;
SET collation_connection = 'utf8mb4_unicode_ci';

-- Criar database IdentityUsers se não existir
CREATE DATABASE IF NOT EXISTS IdentityUsers
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE IdentityUsers;

-- Tabela de papéis (roles)
CREATE TABLE IF NOT EXISTS `AspNetRoles` (
  `Id` varchar(255) NOT NULL,
  `Name` varchar(256) NULL,
  `NormalizedName` varchar(256) NULL,
  `ConcurrencyStamp` longtext NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `RoleNameIndex` (`NormalizedName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Tabela de usuários
CREATE TABLE IF NOT EXISTS `AspNetUsers` (
  `Id` varchar(255) NOT NULL,
  `UserName` varchar(256) NULL,
  `NormalizedUserName` varchar(256) NULL,
  `Email` varchar(256) NULL,
  `NormalizedEmail` varchar(256) NULL,
  `EmailConfirmed` tinyint(1) NOT NULL,
  `PasswordHash` longtext NULL,
  `SecurityStamp` longtext NULL,
  `ConcurrencyStamp` longtext NULL,
  `PhoneNumber` longtext NULL,
  `PhoneNumberConfirmed` tinyint(1) NOT NULL,
  `TwoFactorEnabled` tinyint(1) NOT NULL,
  `LockoutEnd` datetime NULL,
  `LockoutEnabled` tinyint(1) NOT NULL,
  `AccessFailedCount` int NOT NULL,
  `Ativo` tinyint(1) NOT NULL DEFAULT 0,
  `DataCadastro` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `DataNascimento` datetime(6) NULL,
  `NomeCompleto` varchar(100) NULL,
  `UltimoLogin` datetime(6) NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UserNameIndex` (`NormalizedUserName`),
  KEY `EmailIndex` (`NormalizedEmail`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Claims de Roles
CREATE TABLE IF NOT EXISTS `AspNetRoleClaims` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `RoleId` varchar(255) NOT NULL,
  `ClaimType` longtext NULL,
  `ClaimValue` longtext NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_AspNetRoleClaims_RoleId` (`RoleId`),
  CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId`
    FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Claims de Usuários
CREATE TABLE IF NOT EXISTS `AspNetUserClaims` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` varchar(255) NOT NULL,
  `ClaimType` longtext NULL,
  `ClaimValue` longtext NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_AspNetUserClaims_UserId` (`UserId`),
  CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId`
    FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Logins externos
CREATE TABLE IF NOT EXISTS `AspNetUserLogins` (
  `LoginProvider` varchar(128) NOT NULL,
  `ProviderKey` varchar(128) NOT NULL,
  `ProviderDisplayName` longtext NULL,
  `UserId` varchar(255) NOT NULL,
  PRIMARY KEY (`LoginProvider`,`ProviderKey`),
  KEY `IX_AspNetUserLogins_UserId` (`UserId`),
  CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId`
    FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Relação Usuário-Role
CREATE TABLE IF NOT EXISTS `AspNetUserRoles` (
  `UserId` varchar(255) NOT NULL,
  `RoleId` varchar(255) NOT NULL,
  PRIMARY KEY (`UserId`,`RoleId`),
  KEY `IX_AspNetUserRoles_RoleId` (`RoleId`),
  CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId`
    FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId`
    FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Tokens de usuário
CREATE TABLE IF NOT EXISTS `AspNetUserTokens` (
  `UserId` varchar(255) NOT NULL,
  `LoginProvider` varchar(128) NOT NULL,
  `Name` varchar(128) NOT NULL,
  `Value` longtext NULL,
  PRIMARY KEY (`UserId`,`LoginProvider`,`Name`),
  CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId`
    FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Histórico de migrações do EF (marcando como aplicadas)
CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT IGNORE INTO `__EFMigrationsHistory` (`MigrationId`,`ProductVersion`) VALUES
 ('20250908215603_CreateIdentitySchema','8.0.18'),
 ('20250908231347_AddUserProfileFields','8.0.18'),
 ('20250908234117_RemoveTipoUsuario','8.0.18');

-- (Opcional) Usuário seed (descomentando exige hash válido de senha)
-- INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, Ativo, DataCadastro, NomeCompleto)
-- VALUES ('seed-user-1', 'admin', 'ADMIN', 'admin@aluguelink.com', 'ADMIN@ALUGUELINK.COM', 1, '<HASH_AQUI>', UUID(), UUID(), 0, 0, 0, 0, 1, NOW(6), 'Administrador');

-- Retorna ao banco principal se desejar continuar operações nele
USE aluguelink;