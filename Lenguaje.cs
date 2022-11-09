//  Carlos Mallard

//Requerimiento 1.- Actualización: 
//                   a) Agregar el residuo de la división en el porfactor 
//                   b) Agregar en instruccion los incrementos de término y los incrementos de factor 
//                      a++, a--, a+=1, a-=1, a*=1, a/=1, a%=1
//                      en donde el 1 puede ser una expresión 
//                   c) Programar el destructor
//                      para ejecutar el metodo cerrarArchivo
//                      #libreria especial? contenedor?
//                      #en la clase lexico

//Requerimiento 2.- Actualizacion 2
//                   c) Marcar errores semanticos cuando los incrementos de termino
//                      o icrementos de factor superen el rango de la variable
//                   d) Considerar el inciso b) y c) para el for 
//                   e) que funcione el do y el while

//Requerimiento 3.- Agregar:
//                   a) considerar las variables y los casteos de las expresiones matematicas en ensamblador
//                   b) considerar el residuo de la división en ensamblador, el residuo de la division queda en dx 
//                   c) Programar el printf y el scanf en ensamblador 

// Requerimiento 4.- a) Programar el else en ensamblador 
//                   b) Programar el for en ensamblador

// Requerimiento 5.- a) Programar el while en ensamblador
//                   b) Programar el do-while en ensamblador

// Sólo Dios y yo sabemos cómo funciona este código - 08/11/2022
using System;
using System.Collections.Generic;


namespace Semantica
{
    public class Lenguaje : Sintaxis
    {
        List<Variable> variables = new List<Variable>();

        Stack<float> stack = new Stack<float>();

        Variable.TipoDato dominante;

        string incrementoGlobal = "";
        int cIF;
        int cFOR;
        int cWhile;
        int cDo;
        public Lenguaje()
        {
            cIF = cFOR = cWhile = cDo = 0;

        }
        public Lenguaje(string nombre) : base(nombre)
        {
            cIF = cFOR = cWhile = cDo = 0;
        }
        ~Lenguaje()
        {
            Console.WriteLine("\t\nDestructor");
            cerrar();
        }

        private void addVariable(String nombre, Variable.TipoDato tipo)
        {
            variables.Add(new Variable(nombre, tipo));
        }
        private void displayVariables()
        {
            log.WriteLine();
            log.WriteLine("\nvariables: ");
            foreach (Variable v in variables)
            {
                log.WriteLine(v.getNombre() + " " + v.getTipo() + " " + v.getValor());
            }
        }

        private bool existeVariable(string nombre)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombre))
                {
                    return true;
                }
            }
            return false;
        }
        private void modVariable(string nombre, float nuevoValor)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombre))
                {
                    v.setValor(nuevoValor);
                }
            }
        }
        private float getValor(string nombreVariable)
        {
            float var = 0;
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombreVariable))
                {
                    var = v.getValor();
                }
            }
            return var;
        }
        private Variable.TipoDato getTipo(string nombreVariable)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombreVariable))
                {
                    return v.getTipo();
                }
            }
            return Variable.TipoDato.Char; // nomás pa'que no marque error (porque nunca va a pasar, antes se levanta error)
        }

        private void VariablesAssembly()
        {
            asm.WriteLine(";Variables");
            foreach (Variable v in variables)
            {
                switch (v.getTipo())
                {
                    case Variable.TipoDato.Char:
                        asm.WriteLine("\t" + v.getNombre() + " DW ?" + v.getValor());
                        break;
                    case Variable.TipoDato.Int:
                        asm.WriteLine("\t" + v.getNombre() + " DW ?" + v.getValor());
                        break;
                    case Variable.TipoDato.Float:
                        asm.WriteLine("\t" + v.getNombre() + " DW ?" + v.getValor());
                        break;
                }
            }
        }

        private float convert(float valor, string dato)
        {
            switch (dato)
            {
                case "Char":
                    return valor % 256;
                case "Int":
                    return valor % 65535;
                default:
                    return valor;
            }
        }
        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            asm.WriteLine("#make_COM#");
            asm.WriteLine("include emu8086.inc");
            asm.WriteLine("ORG 100h");
            Libreria();
            Variables();
            Main();
            asm.WriteLine("");
            VariablesAssembly();
            displayVariables();
            asm.WriteLine("RET");
            asm.WriteLine("DEFINE_SCAN_NUM");
            asm.WriteLine("DEFINE_PRINT_NUM");
            asm.WriteLine("DEFINE_PRINT_NUM_UNS");
            asm.WriteLine("END");
        }

        //Librerias -> #include<identificador(.h)?> Librerias?
        private void Libreria()
        {
            if (getContenido() == "#")
            {
                match("#");
                match("include");
                match("<");
                match(Tipos.Identificador);
                if (getContenido() == ".")
                {
                    match(".");
                    match("h");
                }
                match(">");
                Libreria();
            }
        }

        //Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variable.TipoDato tipo = Variable.TipoDato.Char;

                switch (getContenido())
                {
                    case "int": tipo = Variable.TipoDato.Int; break;
                    case "float": tipo = Variable.TipoDato.Float; break;
                }

                match(Tipos.TipoDato);
                Lista_identificadores(tipo);
                match(Tipos.FinSentencia);
                Variables();
            }
        }

        //Lista_identificadores -> identificador (,Lista_identificadores)?
        private void Lista_identificadores(Variable.TipoDato tipo)
        {
            if (getClasificacion() == Tipos.Identificador)
            {
                if (!existeVariable(getContenido()))
                {
                    addVariable(getContenido(), tipo);
                }
                else
                {
                    throw new Error("Error de sintaxis, variable duplicada <" + getContenido() + "> en linea: " + linea, log);
                }
            }

            match(Tipos.Identificador);

            if (getContenido() == ",")
            {
                match(",");
                Lista_identificadores(tipo);
            }
        }
        //Main -> void main() Bloque de instrucciones
        private void Main()
        {
            match("void");
            match("main");
            match("(");
            match(")");
            BloqueInstrucciones(true, true);
        }
        //Bloque de instrucciones -> {listaIntrucciones?}
        private void BloqueInstrucciones(bool evaluacion, bool ASM)
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion, ASM);
            }
            match("}");
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool evaluacion, bool ASM)
        {
            Instruccion(evaluacion, ASM);
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion, ASM);
            }
        }

        //ListaInstruccionesCase -> Instruccion ListaInstruccionesCase?
        private void ListaInstruccionesCase(bool evaluacion, bool ASM)
        {
            Instruccion(evaluacion, ASM);
            if (getContenido() != "case" && getContenido() != "break" && getContenido() != "default" && getContenido() != "}")
            {
                ListaInstruccionesCase(evaluacion, ASM);
            }
        }

        //Instruccion -> Printf | Scanf | If | While | do while | For | Switch | Asignacion
        //Funcion que genera numeros aleatorios

        private void Instruccion(bool evaluacion, bool ASM)
        {
            if (getContenido() == "printf")
            {
                Printf(evaluacion, ASM);
            }
            else if (getContenido() == "scanf")
            {
                Scanf(evaluacion, ASM);
            }
            else if (getContenido() == "if")
            {
                If(evaluacion, ASM);
            }
            else if (getContenido() == "while")
            {
                While(evaluacion, ASM);
            }
            else if (getContenido() == "do")
            {
                Do(evaluacion, ASM);
            }
            else if (getContenido() == "for")
            {
                For(evaluacion, ASM);
            }
            else if (getContenido() == "switch")
            {
                Switch(evaluacion, ASM);
            }
            else
            {
                Asignacion(evaluacion, ASM);
            }
        }

        private Variable.TipoDato evaluaNumero(float resultado)
        {
            if (resultado % 1 != 0)
            {
                return Variable.TipoDato.Float;
            }
            if (resultado <= 255)
            {
                return Variable.TipoDato.Char;
            }
            else if (resultado <= 65535)
            {
                return Variable.TipoDato.Int;
            }
            return Variable.TipoDato.Float;
        }
        private bool evaluaSemantica(string variable, float resultado)
        {
            Variable.TipoDato tipoDato = getTipo(variable);
            return false;
        }

        //Asignacion -> identificador = cadena | Expresion;
        private void Asignacion(bool evaluacion, bool ASM)
        {

            log.WriteLine();
            log.Write(getContenido() + " = ");

            string nombre = getContenido();

            if (!existeVariable(getContenido()))
                throw new Error("Error : No existe la variable \'" + getContenido() + "\' en linea: " + linea, log);

            match(Tipos.Identificador);
            dominante = Variable.TipoDato.Char;

            if (getClasificacion() == Tipos.IncrementoTermino || getClasificacion() == Tipos.IncrementoFactor)
            {
                string incrementoTipo = getContenido();

                if (getClasificacion() == Tipos.IncrementoTermino)
                {
                    float resultados = incrementos(nombre, incrementoTipo, ASM);

                    match(";");

                    if (dominante < evaluaNumero(resultados))
                    {
                        dominante = evaluaNumero(resultados);
                    }
                    if (dominante <= getTipo(nombre))
                    {
                        modVariable(nombre, resultados);
                        if (ASM)
                        {
                            asm.WriteLine(incrementoGlobal);
                        }
                    }
                    else
                    {
                        throw new Error("Error de semántico: variable <" + nombre + "> no se le puede asginar el valor <" + resultados + "> en linea " + linea, log);
                    }
                }
                else
                {
                    float resultados = incrementos(nombre, incrementoTipo, ASM);
                    match(";");
                    if (dominante < evaluaNumero(resultados))
                    {
                        dominante = evaluaNumero(resultados);
                    }
                    if (dominante <= getTipo(nombre))
                    {
                        modVariable(nombre, resultados);

                        if (ASM)
                        {
                            asm.WriteLine(incrementoGlobal);
                        }

                    }
                    else
                    {
                        throw new Error("Error semántico: variable <" + nombre + "> no se le puede asiginar el valor <" + resultados + "> en linea " + linea, log);
                    }
                }
            }

            match(Tipos.Asignacion);
            Expresion(ASM);
            match(";");

            float resultado = stack.Pop();

            if (ASM)
            {
                asm.WriteLine("POP AX");
            }

            log.Write("= " + resultado);
            log.WriteLine();

            if (dominante < evaluaNumero(resultado))
            {
                dominante = evaluaNumero(resultado);
            }
            if (dominante <= getTipo(nombre))
            {
                if (evaluacion)
                {
                    modVariable(nombre, resultado);
                }
            }
            else
            {
                throw new Error("Error de semantica: no podemos asignar un: <" + dominante + "> a un <" + getTipo(nombre) + "> en linea " + linea, log);
            }

            if (ASM)
            {
                asm.WriteLine("MOV " + nombre + ", AX");
            }
        }

        //b) Agregar en instruccion los incrementos de término y los incrementos de factor 
        private float incrementos(string Variable, string tipoIncremento, bool ASM)
        {
            float resultado = getValor(Variable);
            if (existeVariable(Variable))
            {
                switch (tipoIncremento)
                {
                    case "++":
                        match("++");
                        incrementoGlobal = "INC " + Variable;
                        resultado++;
                        break;
                    case "--":
                        match("--");
                        incrementoGlobal = "DEC " + Variable;
                        resultado--;
                        break;
                    case "+=":
                        match("+=");
                        Expresion(ASM);
                        incrementoGlobal = "POP AX";
                        incrementoGlobal += "ADD " + Variable + ", AX";
                        incrementoGlobal += "MOV " + Variable + ", AX";
                        resultado += stack.Pop();
                        break;
                    case "-=":
                        match("-=");
                        Expresion(ASM);
                        incrementoGlobal = "POP AX";
                        incrementoGlobal += "SUB " + Variable + ", AX";
                        incrementoGlobal += "MOV " + Variable + ", AX";
                        resultado -= stack.Pop();
                        break;
                    case "*=":
                        match("*=");

                        incrementoGlobal = "POP AX";
                        incrementoGlobal += "MUL " + Variable + ", AX";
                        incrementoGlobal += "MOV " + Variable + ", AX";
                        Expresion(ASM);
                        resultado *= stack.Pop();
                        break;
                    case "/=":
                        match("/=");
                        incrementoGlobal = "POP AX";
                        incrementoGlobal += "DIV " + Variable + ", AX";
                        incrementoGlobal += "MOV " + Variable + ", AX";
                        Expresion(ASM);
                        resultado /= stack.Pop();
                        break;
                    case "%=":
                        match("%=");
                        incrementoGlobal = "POP AX";
                        incrementoGlobal += "MOD " + Variable + ", AX";
                        incrementoGlobal += "MOV " + Variable + ", AX";
                        Expresion(ASM);
                        resultado %= stack.Pop();
                        break;
                }
                return resultado;
            }
            return 0;

        }
        // While -> while(Condicion) bloque de instrucciones | instruccion
        private void While(bool evaluacion, bool ASM)
        {
            match("while");
            match("(");

            bool validarWhile;

            int posicionGuardada = contador;
            int lineaGuardada = linea;

            string variable = getContenido();

            string etiquetaIinicnioWHILE = "WHILE" + cWhile + ":";
            string etiquetaFinWHILE = "FINWHILE" + cWhile + ":";

            if (ASM)
            {
                asm.WriteLine(etiquetaIinicnioWHILE);
            }

            do
            {
                validarWhile = Condicion(etiquetaIinicnioWHILE, ASM);

                if (!evaluacion)
                {
                    validarWhile = false;
                }
                // Requerimiento 4
                match(")");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(validarWhile, ASM);
                }
                else
                {
                    Instruccion(evaluacion, ASM);
                }
                if (validarWhile)
                {
                    contador = posicionGuardada - variable.Length;
                    linea = lineaGuardada;

                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(contador, SeekOrigin.Begin);

                    NextToken();
                }
                if (ASM)
                {
                    asm.WriteLine("JMP " + etiquetaIinicnioWHILE);
                    asm.WriteLine(etiquetaFinWHILE);
                }
            } while (validarWhile);
        }

        // Do -> do bloque de instrucciones | intruccion while(Condicion)
        private void Do(bool evaluacion, bool ASM)
        {
            match("do");

            bool validarDo = Condicion("", ASM);

            int posicionGuardada = contador;
            int lineaGuardada = linea;

            string etiquetaIinicnioDO = "DO" + cDo + ":";
            string etiquetaFinDO = "FINDO" + cDo + ":";

            if (ASM)
            {
                asm.WriteLine(etiquetaIinicnioDO);
            }

            do
            {
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(evaluacion, ASM);
                }
                else
                {
                    Instruccion(evaluacion, ASM);
                }
                match("while");
                match("(");

                string variable = getContenido();
                validarDo = Condicion(etiquetaFinDO, ASM);
                // Requerimiento 4
                if (!evaluacion)
                {
                    validarDo = false;
                }
                match(")");
                match(";");

                if (validarDo)
                {
                    contador = posicionGuardada - variable.Length;
                    linea = lineaGuardada;

                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(contador, SeekOrigin.Begin);

                    NextToken();
                }
                if (ASM)
                {
                    asm.WriteLine("JMP " + etiquetaIinicnioDO);
                    asm.WriteLine(etiquetaFinDO);
                }
            } while (validarDo);
        }
        // For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
        private void For(bool evaluacion, bool ASM)
        {
            bool validarFor;

            if (ASM)
            {
                cFOR++;
            }

            string etiquetaInicioFOR = "inicioFor" + cFOR;
            string etiquetaFinFOR = "finFor" + cFOR;
            incrementoGlobal = "";

            match("for");
            match("(");
            Asignacion(evaluacion, ASM);
            // Requerimiento 4
            // Requerimiento 6:
            // a) Necesito guardar la posición del archivo del texto

            int posicionGuardada = contador;
            int lineaGuardada = linea;

            string variable = getContenido();
            float valor = getValor(variable);
            do
            {
                if (ASM)
                {
                    asm.WriteLine(etiquetaInicioFOR + ":");
                }

                validarFor = Condicion(etiquetaFinFOR, ASM);

                if (!evaluacion)
                {
                    validarFor = evaluacion;
                }
                // b) Agregar un ciclo while, después del validarFor() * 

                match(";");
                match(Tipos.Identificador);
                float sumaresta = incrementos(variable, getContenido(), ASM);
                match(")");

                if (getContenido() == "{")
                {
                    BloqueInstrucciones(validarFor, ASM);
                }
                else
                {
                    Instruccion(validarFor, ASM);
                }
                // c) Regresar a la posicion de lectura del archivo
                // d) Sacar otro valor de la pila
                if (validarFor)
                {
                    if (sumaresta == 1)
                    {
                        modVariable(variable, getValor(variable) + 1);
                    }
                    else if (sumaresta == -1)
                    {
                        modVariable(variable, getValor(variable) - 1);
                    }
                    contador = posicionGuardada - variable.Length;
                    linea = lineaGuardada;

                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(contador, SeekOrigin.Begin);

                    NextToken();
                }

                if (ASM)
                {
                    asm.WriteLine(incrementoGlobal);
                    asm.WriteLine("JMP " + etiquetaInicioFOR);
                    asm.WriteLine(etiquetaFinFOR + ":");
                }
            }
            while (validarFor);
            //asm.WriteLine(etquietaFinFOR + ":");
        }

        //Incremento -> Identificador ++ | --
        private void incremento(bool evaluacion, bool ASM)
        {
            string variable = getContenido();
            //Requerimiento 2.- Si no existe la variable levanta la excepcion
            if (!existeVariable(getContenido()))
                throw new Error("Error : No existe variable \'" + getContenido() + "\' en linea: " + linea, log);

            match(Tipos.Identificador);

            if (getContenido() == "++")
            {
                if (evaluacion)
                {
                    modVariable(variable, getValor(variable) + 1);
                }
            }
            if (getContenido() == "--")
            {

                if (evaluacion)
                {
                    modVariable(variable, getValor(variable) - 1);
                }
            }
        }

        //Switch -> switch (Expresion) {Lista de casos} | (default: )
        private void Switch(bool evaluacion, bool ASM)
        {
            match("switch");
            match("(");
            Expresion(ASM);
            stack.Pop();

            if (ASM)
            {
                asm.WriteLine("POP AX");
            }

            match(")");
            match("{");
            ListaDeCasos(evaluacion, ASM);
            if (getContenido() == "default")
            {
                match("default");
                match(":");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(evaluacion, ASM);
                }
                else
                {
                    Instruccion(evaluacion, ASM);
                }
            }
            match("}");
        }

        //ListaDeCasos -> case Expresion: listaInstruccionesCase (break;)? (ListaDeCasos)?
        private void ListaDeCasos(bool evaluacion, bool ASM)
        {
            match("case");
            Expresion(ASM);
            stack.Pop();
            asm.WriteLine("POP AX");
            match(":");
            ListaInstruccionesCase(evaluacion, ASM);
            if (getContenido() == "break")
            {
                match("break");
                match(";");
            }
            if (getContenido() == "case")
            {
                ListaDeCasos(evaluacion, ASM);
            }
        }

        //Condicion -> Expresion operador relacional Expresion
        private bool Condicion(string etiqueta, bool ASM)
        {
            Expresion(ASM);
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion(ASM);

            float e2 = stack.Pop();
            if (ASM)
            {
                asm.WriteLine("POP AX");
            }
            float e1 = stack.Pop();
            if (ASM)
            {
                asm.WriteLine("POP BX");
                asm.WriteLine("CMP AX, BX");
            }

            switch (operador)
            {
                case "==":
                    if (ASM)
                    {
                        asm.WriteLine("JNE " + etiqueta);
                    }

                    return e1 == e2;
                case ">":
                    if (ASM)
                    {
                        asm.WriteLine("JLE " + etiqueta);
                    }
                    return e1 > e2;
                case "<":
                    if (ASM)
                    {
                        asm.WriteLine("JGE " + etiqueta);
                    }
                    return e1 < e2;
                case ">=":
                    if (ASM)
                    {
                        asm.WriteLine("JL " + etiqueta);
                    }
                    return e1 >= e2;
                case "<=":
                    if (ASM)
                    {
                        asm.WriteLine("JG " + etiqueta);
                    }
                    return e1 <= e2;
                default:
                    if (ASM)
                    {
                        asm.WriteLine("JE " + etiqueta);
                    }
                    return e1 != e2;
            }
        }

        //If -> if(Condicion) bloque de instrucciones (else bloque de instrucciones)?
        private void If(bool evaluacion, bool ASM)
        {
            
            if (ASM)
            {
                cIF++;
            }
            string etquietaIF = "if" + cIF;
            string etquietaELSE = "else" + cIF;
            string etiquetaFINIF = "fin" + cIF;

            match("if");
            match("(");

            bool validarIf = Condicion(etquietaIF, ASM);
            if (!evaluacion)
            {
                validarIf = false;
            }
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(validarIf, ASM);
            }
            else
            {
                Instruccion(validarIf, ASM);
            }

            if (ASM)
            {
                asm.WriteLine("JMP " + etiquetaFINIF);
            }

            if (getContenido() == "else")
            {
                match("else");
                if (ASM)
                {
                    asm.WriteLine(etquietaELSE + ":");
                }
                // Requerimiento 4
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(!validarIf, ASM);
                }
                else
                {
                    Instruccion(!validarIf, ASM);
                }
                if (ASM)
                {
                    asm.WriteLine("JMP" + etiquetaFINIF);
                }
            }
            if (ASM)
            {

                asm.WriteLine(etquietaIF + ":");
                asm.WriteLine("JMP Fin " + etquietaELSE);
                asm.WriteLine(etiquetaFINIF + ":");
            }
        }

        //Printf -> printf(cadena|expresion);
        private void Printf(bool evaluacion, bool ASM)
        {
            match("printf");
            match("(");
            if (getClasificacion() == Tipos.Cadena)
            {
                // Requerimiento 1 *
                if (evaluacion)
                {
                    setContenido(getContenido().Substring(1, getContenido().Length - 2));
                    setContenido(getContenido().Replace("\\t", "\t"));
                    setContenido(getContenido().Replace("\\n", "\n"));
                    setContenido(getContenido().Replace("\\\t", "\\t"));
                    setContenido(getContenido().Replace("\\\n", "\\n"));
                    Console.Write(getContenido());

                    if (ASM)
                    {
                        asm.WriteLine("PRINTN \"" + getContenido() + "\"");
                    }
                }

                match(Tipos.Cadena);
            }
            else
            {
                Expresion(ASM);
                float resultado = stack.Pop();
                if (ASM)
                {
                    asm.WriteLine("POP AX");
                    asm.WriteLine("PRINT_NUM AX");
                }
                if (evaluacion)
                {
                    Console.Write(stack.Pop());
                    // TODO Codigo ensamblador para imprimir una variable
                }
            }
            match(")");
            match(";");
        }
        //Scanf -> scanf(cadena, &Identificador);
        private void Scanf(bool evaluacion, bool ASM)
        {
            match("scanf");
            match("(");
            match(Tipos.Cadena);
            match(",");
            match("&");
            // Requerimiento 2.- Si no existe la variable levanta la excepcion *
            if (!existeVariable(getContenido()))
                throw new Error("Error : No existe variable \'" + getContenido() + "\' en linea: " + linea, log);
            // Requerimiento 5.- Modificar el valor de la variable *
            string nombreVariable = getContenido();
            match(Tipos.Identificador);
            if (evaluacion)
            {
                string val = "" + Console.ReadLine();
                if (esNumero(val))
                {
                    modVariable(getContenido(), float.Parse(val));
                }
                else
                {
                    throw new Error("Error de sintaxis: la variable debe ser un numero. \'" + val + "\' no es un numero. Error en linea: " + linea, log);
                }
            }
            match(")");
            match(";");

            if
                (ASM)
            {
                asm.WriteLine("CALL SCAN_NUM");
                asm.WriteLine("MOV " + getContenido() + ", CX");
            }

        }

        //método que verifica si es numero en un try catch
        private bool esNumero(string s)
        {
            bool resultado = false;
            try
            {
                float flotante = float.Parse(s);
                resultado = true;
            }
            catch { }

            return resultado;
        }
        //Expresion -> Termino MasTermino
        private void Expresion(bool ASM)
        {

            Termino(ASM);
            MasTermino(ASM);
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino(bool ASM)
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino(ASM);
                log.Write(operador + " ");

                float n1 = stack.Pop();
                if (ASM)
                {
                    asm.WriteLine("POP AX");
                }

                float n2 = stack.Pop();
                if (ASM)
                {
                    asm.WriteLine("POP BX");
                }

                switch (operador)
                {
                    case "+":
                        stack.Push(n2 + n1);
                        if (ASM)
                        {
                            asm.WriteLine("ADD AX, BX");
                            asm.WriteLine("PUSH AX");
                        }

                        break;
                    case "-":
                        stack.Push(n2 - n1);
                        if (ASM)
                        {
                            asm.WriteLine("SUB AX, BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;

                    case "*":
                        stack.Push(n2 * n1);
                        if (ASM)
                        {
                            asm.WriteLine("MUL AX, BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                    case "/":
                        stack.Push(n2 / n1);
                        if (ASM)
                        {
                            asm.WriteLine("DIV AX, BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                    case "%":
                        stack.Push(n2 % n1);
                        if (ASM)
                        {
                            asm.WriteLine("MOD AX, BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                    case "^":
                        stack.Push((float)Math.Pow(n2, n1));
                        if (ASM)
                        {
                            asm.WriteLine("POW AX, BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino(bool ASM)
        {
            Factor(ASM);
            PorFactor(ASM);
        }
        //PorFactor -> (OperadorFactor Factor)? 
        private void PorFactor(bool ASM)
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor(ASM);
                log.Write(operador + " ");
                float n1 = stack.Pop();
                if (ASM)
                {
                    asm.WriteLine("POP AX");
                }
                float n2 = stack.Pop();
                if (ASM)
                {
                    asm.WriteLine("POP BX");
                }
                //Requerimiento 1 a)
                switch (operador)
                {
                    case "*":
                        stack.Push(n2 * n1);
                        if (ASM)
                        {
                            asm.WriteLine("MUL BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;

                    case "/":
                        //Requerimiento 1 a)
                        if (n1 != 0)
                        {
                            //Obtener residuo como un número 
                            stack.Push(n2 / n1);
                            if (ASM)
                            {
                                asm.WriteLine("DIV BX");
                                asm.WriteLine("PUSH AX");
                            }
                        }
                        else
                        {
                            throw new Error("Error de sintaxis: hay división entre cero en linea  " + linea, log);
                        }
                        break;
                    case "%":
                        stack.Push(n2 % n1);
                        if (ASM)
                        {
                            asm.WriteLine("DIV BX");
                            asm.WriteLine("PUSH DX");
                        }
                        break;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor(bool ASM)
        {
            if (getClasificacion() == Tipos.Numero)
            {
                log.Write(getContenido() + " ");
                if (dominante < evaluaNumero(float.Parse(getContenido())))
                {
                    dominante = evaluaNumero(float.Parse(getContenido()));
                }
                stack.Push(float.Parse(getContenido()));

                if (ASM)
                {
                    asm.WriteLine("MOV AX," + getContenido());
                    asm.WriteLine("PUSH AX");
                }

                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                if (!existeVariable(getContenido()))
                    throw new Error("Error : No existe variable \'" + getContenido() + "\' en linea: " + linea, log);

                log.Write(getContenido() + " ");
                string variable = getContenido();

                // Requerimiento 1
                if (dominante < getTipo(getContenido()))
                {
                    dominante = getTipo(getContenido());
                }

                stack.Push(getValor(getContenido()));

                if (ASM)
                {
                    asm.WriteLine("MOV AX," + variable);
                    asm.WriteLine("PUSH AX");
                }
                match(Tipos.Identificador);
                // Requerimiento 3 a)
                // pasamos al siguiente token 

            }
            else
            {
                bool huboCasteo = false;
                Variable.TipoDato casteo = Variable.TipoDato.Char;
                match("(");
                if (getClasificacion() == Tipos.TipoDato)
                {
                    huboCasteo = true;
                    switch (getContenido())
                    {
                        case "char":
                            casteo = Variable.TipoDato.Char;
                            break;
                        case "int":
                            casteo = Variable.TipoDato.Int;
                            break;
                        case "float":
                            casteo = Variable.TipoDato.Float;
                            break;
                    }
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                }
                Expresion(ASM);
                match(")");
                if (huboCasteo)
                {
                    dominante = casteo;
                    float variable = stack.Pop();

                    switch (casteo)
                    {
                        case Variable.TipoDato.Char:
                            if (ASM)
                            {
                                asm.WriteLine("POP AX");
                                asm.WriteLine("MOV AL, AH");
                                asm.WriteLine("PUSH AX");
                            }
                            break;
                        case Variable.TipoDato.Int:
                            if (ASM)
                            {
                                asm.WriteLine("POP AX");
                                asm.WriteLine("MOV AH, 0");
                                asm.WriteLine("PUSH AX");
                                asm.WriteLine("MOD AX, BX");
                                asm.WriteLine("PUSH AX");
                            }
                            break;
                        case Variable.TipoDato.Float:
                            if (ASM)
                            {
                                asm.WriteLine("POP AX");
                                asm.WriteLine("MOV AH, 0");
                                asm.WriteLine("PUSH AX");
                                asm.WriteLine("MOD AX, BX");
                                asm.WriteLine("PUSH AX");
                            }
                            break;
                    }

                    stack.Push(convert(variable, casteo.ToString()));

                    // Requerimiento 2.- Saco un elemento del stack
                    //                   Convierto ese valor al equialente en casteo
                    // Requerimiento 3.- Ej. Si el casteo es (char) y el pop regresa un 256
                    //                   el valor equivalente en casteo es 0
                }
            }
        }
    }
}