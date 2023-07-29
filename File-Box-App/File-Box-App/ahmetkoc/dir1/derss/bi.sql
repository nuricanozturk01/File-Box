
DROP DATABASE IF EXISTS yediiklim;
CREATE DATABASE yediiklim;
USE yediiklim;

CREATE TABLE fund_flow
(
    fund_flow_pk_id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    date DATE NOT NULL,
    description VARCHAR(80) NOT NULL,
    debt DECIMAL(19, 2) NOT NULL,
    exchangeRate DOUBLE NOT NULL,
    currency VARCHAR(10) NOT NULL,
    resultAmount DECIMAL(19,2)
);

CREATE TABLE other_flow -- ŞİRKETE AİT OLMAYAN GİDERLER
(
    other_flow_pk_id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    date DATE NOT NULL,
    description VARCHAR(80) NOT NULL,
    debt DECIMAL(19, 2) NOT NULL,
    exchangeRate DOUBLE NOT NULL,
    currency VARCHAR(10) NOT NULL,
    resultAmount DECIMAL(19,2)
);

CREATE TABLE treasure_flow -- GİRDİLER VE MEVCUT KIYMETLER
(
    treasure_flow_pk_id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    date DATE NOT NULL,
    description VARCHAR(80) NOT NULL,
    debt DECIMAL(19, 2) NOT NULL,
    exchangeRate DOUBLE NOT NULL,
    currency VARCHAR(10) NOT NULL,
    resultAmount DECIMAL(19,2)
);
CREATE TABLE date_mapper
(
    mapper_pk_id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    m_mon INT NOT NULL,
    m_year INT NOT NULL
);

CREATE TABLE cost_type
(
    cost_type_pk_id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    costType VARCHAR(45) UNIQUE NOT NULL
);
CREATE TABLE kdv
(
    kdv_pk_id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    kdv DOUBLE NOT NULL
);
CREATE TABLE user
(
    user_pk_id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(45) NOT NULL,
    surname VARCHAR(45) NOT NULL,
    amount DECIMAL(19,2) DEFAULT 0.00
);

CREATE TABLE off_day_types
(
    off_day_types_pk_id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    type VARCHAR(50) NOT NULL UNIQUE
);


CREATE TABLE advance_amount
(
    advance_pk_id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    user_pk_id INT NOT NULL,
    amount DECIMAL(19,2) NOT NULL,
    amount_date DATE NOT NULL,
    -- transfer TINYINT DEFAULT FALSE,
    -- amount_year INT NOT NULL,
    -- amount_month INT NOT NULL,
    CONSTRAINT FOREIGN KEY (user_pk_id) REFERENCES User(user_pk_id) ON UPDATE CASCADE
);

CREATE TABLE off_days
(
    off_day_pk_id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    user_pk_id INT NOT NULL,
    off_day_date DATE NOT NULL,
    off_day_type VARCHAR(45) NOT NULL,
    CONSTRAINT FOREIGN KEY (user_pk_id) REFERENCES User(user_pk_id) ON UPDATE CASCADE
);
CREATE TABLE cost_form
(
    cost_form_pk_id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    user_pk_id INT NOT NULL,
    date DATE NOT NULL,
    costType VARCHAR(45) NOT NULL,
    billingNumber VARCHAR(45),
    description TEXT NOT NULL,
    invoiceAmount DECIMAL(19,2),
    kdv double NOT NULL,
    totalInvoice DECIMAL(19,2) NOT NULL,
    expenditureOfficer VARCHAR(45) NOT NULL,
    company VARCHAR(255),
    isVoucher BIT,
    time TIME,
    CONSTRAINT FOREIGN KEY (user_pk_id) REFERENCES user(user_pk_id) ON UPDATE CASCADE
);
-- DROP TABLE bank_flow;
CREATE TABLE bank_flow
(
    bank_flow_pk_id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
    billingNumber VARCHAR(20) DEFAULT "N/A",
    caseFlow VARCHAR(255),
    cost DECIMAL(19,2),
    date DATE NOT NULL,
    costType VARCHAR(255),
    time TIME
);





DELIMITER ;;
CREATE PROCEDURE filterByDateBankFlow(in _from DATE, in _to DATE)
BEGIN
    SELECT * FROM bank_flow WHERE date BETWEEN _from and _to;
END ;;
DELIMITER ;

DELIMITER ;;
CREATE PROCEDURE filterByDateCostForm(in _from DATE, in _to DATE)
BEGIN
    SELECT * FROM cost_form WHERE date BETWEEN _from and _to;
END ;;
DELIMITER ;

DELIMITER ;;
CREATE PROCEDURE filterByCostTypeCostForm(in cost_type VARCHAR(255))
BEGIN
    SELECT * FROM cost_form WHERE costType = cost_type;
END ;;
DELIMITER ;

DELIMITER ;;
CREATE PROCEDURE filterByCostTypeBankFlow(in cost_type VARCHAR(255))
BEGIN
    SELECT * FROM bank_flow WHERE costType = cost_type;
END ;;
DELIMITER ;

DELIMITER ;;
CREATE PROCEDURE filterByNameCostForm(in p_name VARCHAR(255))
BEGIN
    SELECT * FROM cost_form WHERE p_name = description OR LOCATE(p_name, description) OR p_name = costType OR LOCATE(p_name, costType) > 0;
END ;;
DELIMITER ;

DELIMITER ;;
CREATE PROCEDURE filterByNameBankFlow(in p_name VARCHAR(255))
BEGIN
    SELECT * FROM bank_flow WHERE p_name = caseFlow OR LOCATE(p_name, caseFlow) OR p_name = costType OR LOCATE(p_name, costType) > 0;
END ;;
DELIMITER ;

-- contains eklenebilir
DELIMITER ;;
CREATE PROCEDURE filterByNameUser(in p_name VARCHAR(255))
BEGIN
    SELECT * FROM user WHERE p_name = name OR p_name = surname;
END ;;
DELIMITER ;

-- tam eşleşme ?
DELIMITER ;;
CREATE PROCEDURE filterByNameCostType(in p_name VARCHAR(255))
BEGIN
    SELECT * FROM cost_type WHERE LOCATE(p_name, costType) > 0;
END ;;
DELIMITER ;

DELIMITER ;;
CREATE PROCEDURE filterByNameCostTypeExactMatch(in p_name VARCHAR(255))
BEGIN
    SELECT * FROM cost_type WHERE costType = p_name;
END ;;
DELIMITER ;

DELIMITER ;;
CREATE PROCEDURE getYearByUser(in u_id INT)
BEGIN
    SELECT DISTINCT YEAR(date) FROM cost_form WHERE u_id = user_pk_id ORDER BY YEAR(date);
END ;;
DELIMITER ;

DELIMITER ;;
CREATE PROCEDURE getSumOfCostFormByYear(in p_year INT, in u_id INT)
BEGIN
    SELECT SUM(totalInvoice) FROM cost_form WHERE p_year = YEAR(date) AND u_id = user_pk_id;
END ;;
DELIMITER ;

DELIMITER ;;
CREATE PROCEDURE getDatesGrouped(in u_id INT)
BEGIN
    INSERT INTO date_mapper(m_mon, m_year) SELECT DISTINCT CONVERT(MONTH(date),DECIMAL), CONVERT(YEAR(date), DECIMAL) FROM cost_form WHERE user_pk_id = u_id;
END ;;
DELIMITER ;

DELIMITER ;;
CREATE PROCEDURE getCostFormsWithMonth(in m_month INT, in m_year INT, in u_id INT)
BEGIN
    SELECT * FROM cost_form WHERE MONTH(date) = m_month AND YEAR(date) = m_year AND u_id = user_pk_id;
END ;;
DELIMITER ;

DELIMITER ;;
CREATE PROCEDURE getOffDays(in u_id INT, in p_year INT, in p_month INT)
BEGIN
    SELECT * FROM off_days WHERE MONTH(off_day_date) = p_month AND YEAR(off_day_date) = p_year AND user_pk_id = u_id;
END ;;
DELIMITER ;

DELIMITER ;;
CREATE PROCEDURE getSumOfTypes(in p_type VARCHAR(65))
BEGIN
    SELECT SUM(totalInvoice) FROM cost_form WHERE costType = p_type;
END ;;
DELIMITER ;


DELIMITER ;;
CREATE PROCEDURE getSumOfTypesWithMonthAndYear(in p_type VARCHAR(65), in p_month INT, in p_year INT)
BEGIN
    SELECT SUM(totalInvoice) FROM cost_form WHERE costType = p_type AND MONTH(date) = p_month AND YEAR(date) = p_year;
END ;;
DELIMITER ;

DELIMITER ;;
CREATE PROCEDURE getSumOfAllTypes()
BEGIN
    SELECT SUM(totalInvoice) FROM cost_form;
END ;;
DELIMITER ;

DELIMITER ;;
CREATE PROCEDURE getSumofAllTypesBankFlow()
BEGIN
    SELECT SUM(cost) FROM bank_flow;
END ;;
DELIMITER ;

DELIMITER ;;
CREATE PROCEDURE getOffDaysByYearAndUser(in p_year INT, in p_uid INT)
BEGIN
    SELECT * FROM off_days WHERE user_pk_id = p_uid AND p_year = YEAR(off_day_date);
END ;;
DELIMITER ;

DELIMITER ;;
CREATE PROCEDURE getAdvanceAmounts(in u_id INT, in a_year INT, in a_month INT)
BEGIN
    SELECT * FROM advance_amount WHERE YEAR(amount_date) = a_year AND MONTH(amount_date) = a_month AND user_pk_id = u_id;
END ;;
DELIMITER ;



DELIMITER ;;
CREATE PROCEDURE getAdvanceAmountByDate(in u_id INT, in a_year INT, in a_month INT, in a_day INT)
BEGIN
    SELECT * FROM advance_amount WHERE YEAR(amount_date) = a_year
                                   AND MONTH(amount_date) = a_month AND DAY(amount_date) = a_day AND user_pk_id = u_id;
END ;;
DELIMITER ;


DELIMITER ;;
CREATE PROCEDURE getSumOfTypesBankFlow(in p_type VARCHAR(45))
BEGIN
    SELECT SUM(cost) FROM bank_flow WHERE costType = p_type;
END ;;

DELIMITER ;;
CREATE PROCEDURE getSumOfCostsofPersonWithMonth(in u_id INT, in p_year INT, in p_month INT)
BEGIN
    SELECT SUM(totalInvoice) FROM cost_form WHERE u_id = user_pk_id AND p_year = YEAR(date) AND p_month = MONTH(date);
END ;;

DELIMITER ;;
CREATE PROCEDURE getSumOfTypesBankFlowWithMonth(in p_type VARCHAR(45), in p_month INT)
BEGIN
    SELECT SUM(cost) FROM bank_flow WHERE costType = p_type AND MONTH(date) = p_month;
END ;;
DELIMITER ;


DELIMITER ;;
CREATE PROCEDURE getSumOfTypesBankFlowWithMonthAndYear(in p_type VARCHAR(45), in p_year INT, in p_month INT)
BEGIN
    SELECT SUM(cost) FROM bank_flow WHERE costType = p_type AND MONTH(date) = p_month AND YEAR(date) = p_year;
END ;;
DELIMITER ;

DELIMITER ;;
CREATE PROCEDURE deleteOffDays(in u_id INT, in start_date DATE, in stop_date DATE)
BEGIN
    SET SQL_SAFE_UPDATES = 0;
    DELETE FROM off_days WHERE user_pk_id = u_id AND off_day_date BETWEEN start_date AND stop_date;
    SET SQL_SAFE_UPDATES = 1;
END ;;
DELIMITER ;

DELIMITER ;;
CREATE PROCEDURE getCostType(in p_type VARCHAR(85))
BEGIN
    SELECT * FROM cost_type WHERE costType = p_type;
END ;;
DELIMITER ;

DELIMITER $$
CREATE TRIGGER updateCostFormsForCostTypes AFTER UPDATE
    ON cost_type FOR EACH ROW
BEGIN
    SET SQL_SAFE_UPDATES = 0;
    UPDATE cost_form SET costType = NEW.costType WHERE costType = OLD.costType;
    SET SQL_SAFE_UPDATES = 1;
END$$
DELIMITER ;

DELIMITER $$
CREATE TRIGGER updateCostFormsForBankFlows AFTER UPDATE
    ON cost_type FOR EACH ROW
BEGIN
    SET SQL_SAFE_UPDATES = 0;
    UPDATE bank_flow SET costType = NEW.costType WHERE costType = OLD.costType;
    SET SQL_SAFE_UPDATES = 1;
END$$
DELIMITER ;






-- -------------------------------------------------------------------------------

DELIMITER ;;
CREATE PROCEDURE deleteCostFormsByDateRange(in start_date DATE, in stop_date DATE)
BEGIN
    SET SQL_SAFE_UPDATES = 0;
    DELETE FROM cost_form WHERE date BETWEEN start_date AND stop_date;
    SET SQL_SAFE_UPDATES = 1;
END ;;
DELIMITER ;

DELIMITER ;;
CREATE PROCEDURE deleteBankFlowsByDateRange(in start_date DATE, in stop_date DATE)
BEGIN
    SET SQL_SAFE_UPDATES = 0;
    DELETE FROM bank_flow WHERE date BETWEEN start_date AND stop_date;
    SET SQL_SAFE_UPDATES = 1;
END ;;
DELIMITER ;


DELIMITER ;;
CREATE PROCEDURE getAdvanceAmountsByYear(in u_id INT, in a_year INT)
BEGIN
    SELECT * FROM advance_amount WHERE YEAR(amount_date) = a_year AND user_pk_id = u_id;
END ;;
DELIMITER ;



DELIMITER $$
CREATE TRIGGER updateCostFormOfficerWhenUpdatedUser AFTER UPDATE
    ON user FOR EACH ROW
BEGIN
    SET @newFullname = CONCAT(NEW.name, ' ', NEW.surname);
    SET @oldFullname = CONCAT(OLD.name, ' ', OLD.surname);

    SET SQL_SAFE_UPDATES = 0;
    UPDATE cost_form SET expenditureOfficer = @newFullName WHERE expenditureOfficer = @oldFullName;
    SET SQL_SAFE_UPDATES = 1;
END$$
DELIMITER ;


select * from user;
ALTER TABLE bank_flow AUTO_INCREMENT = 1;