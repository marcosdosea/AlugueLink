-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
-- -----------------------------------------------------
-- Schema aluguelink
-- -----------------------------------------------------

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
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `aluguelink`.`imovel`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aluguelink`.`imovel` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `cep` VARCHAR(8) NOT NULL,
  `logradouro` VARCHAR(100) NOT NULL,
  `numero` VARCHAR(5) NOT NULL,
  `complemento` VARCHAR(50) NULL,
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
  `locador_id` INT NOT NULL,
  PRIMARY KEY (`id`, `locador_id`),
  INDEX `fk_imovel_locador_idx` (`locador_id` ASC) VISIBLE,
  CONSTRAINT `fk_imovel_locador`
    FOREIGN KEY (`locador_id`)
    REFERENCES `aluguelink`.`locador` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `aluguelink`.`locatario`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aluguelink`.`locatario` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `nome` VARCHAR(100) NOT NULL,
  `email` VARCHAR(100) NOT NULL,
  `telefone` VARCHAR(20) NOT NULL,
  `cpf` CHAR(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `cpf_UNIQUE` (`cpf` ASC) VISIBLE)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `aluguelink`.`aluguel`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aluguelink`.`aluguel` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `dataInicio` DATE NOT NULL,
  `dataFim` DATE NOT NULL,
  `status` ENUM('A', 'F', 'P') NULL,
  `dataAssinatura` DATE NOT NULL,
  `locatario_id` INT NOT NULL,
  `imovel_id` INT NOT NULL,
  `imovel_locador_id` INT NOT NULL,
  PRIMARY KEY (`id`, `locatario_id`, `imovel_id`, `imovel_locador_id`),
  INDEX `fk_aluguel_locatario1_idx` (`locatario_id` ASC) VISIBLE,
  INDEX `fk_aluguel_imovel1_idx` (`imovel_id` ASC, `imovel_locador_id` ASC) VISIBLE,
  CONSTRAINT `fk_aluguel_locatario1`
    FOREIGN KEY (`locatario_id`)
    REFERENCES `aluguelink`.`locatario` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_aluguel_imovel1`
    FOREIGN KEY (`imovel_id` , `imovel_locador_id`)
    REFERENCES `aluguelink`.`imovel` (`id` , `locador_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `aluguelink`.`pagamento`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aluguelink`.`pagamento` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `valor` DECIMAL(10,2) NOT NULL,
  `dataPagamento` DATE NOT NULL,
  `tipoPagamento` ENUM('CD', 'CC', 'P', 'B') NOT NULL,
  `aluguel_id` INT NOT NULL,
  PRIMARY KEY (`id`, `aluguel_id`),
  INDEX `fk_pagamento_aluguel1_idx` (`aluguel_id` ASC) VISIBLE,
  CONSTRAINT `fk_pagamento_aluguel1`
    FOREIGN KEY (`aluguel_id`)
    REFERENCES `aluguelink`.`aluguel` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `aluguelink`.`manutencao`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aluguelink`.`manutencao` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `descricao` VARCHAR(200) NOT NULL,
  `dataSolicitacao` DATE NOT NULL,
  `status` ENUM('P', 'A', 'C') NULL,
  `valor` DECIMAL(10,2) NOT NULL,
  `imovel_id` INT NOT NULL,
  `imovel_locador_id` INT NOT NULL,
  PRIMARY KEY (`id`, `imovel_id`, `imovel_locador_id`),
  INDEX `fk_manutencao_imovel1_idx` (`imovel_id` ASC, `imovel_locador_id` ASC) VISIBLE,
  CONSTRAINT `fk_manutencao_imovel1`
    FOREIGN KEY (`imovel_id` , `imovel_locador_id`)
    REFERENCES `aluguelink`.`imovel` (`id` , `locador_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `aluguelink`.`locatario_has_locador`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `aluguelink`.`locatario_has_locador` (
  `locatario_id` INT NOT NULL,
  `locador_id` INT NOT NULL,
  PRIMARY KEY (`locatario_id`, `locador_id`),
  INDEX `fk_locatario_has_locador_locador1_idx` (`locador_id` ASC) VISIBLE,
  INDEX `fk_locatario_has_locador_locatario1_idx` (`locatario_id` ASC) VISIBLE,
  CONSTRAINT `fk_locatario_has_locador_locatario1`
    FOREIGN KEY (`locatario_id`)
    REFERENCES `aluguelink`.`locatario` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_locatario_has_locador_locador1`
    FOREIGN KEY (`locador_id`)
    REFERENCES `aluguelink`.`locador` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
