//  Carlos Mallard

//                   c) Programar el printf y el scanf en ensamblador 
// Requerimiento 4.- a) Programar el else en ensamblador 
//                   b) Programar el for en ensamblador
// Requerimiento 5.- a) Programar el while en ensamblador
//                   b) Programar el do-while en ensamblador
using System;
using System.Collections.Generic;



namespace Semantica
{
    public class Lenguaje : Sintaxis
    {
        List<Variable> variables = new List<Variable>();

        Stack<float> stack = new Stack<float>();

        Variable.TipoDato dominante;

        int cIF;
        int cFOR;
        public Lenguaje()
        {
            cIF = cFOR = 0;

        }
        public Lenguaje(string nombre) : base(nombre)
        {
            cIF = cFOR = 0;
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
            return var; // Al final del foreach 
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
            Libreria();
            Variables();
            Main();
            displayVariables();
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
            BloqueInstrucciones(true);
        }
        //Bloque de instrucciones -> {listaIntrucciones?}
        private void BloqueInstrucciones(bool evaluacion)
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion);
            }
            match("}");
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool evaluacion)
        {
            Instruccion(evaluacion);
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion);
            }
        }

        //ListaInstruccionesCase -> Instruccion ListaInstruccionesCase?
        private void ListaInstruccionesCase(bool evaluacion)
        {
            Instruccion(evaluacion);
            if (getContenido() != "case" && getContenido() != "break" && getContenido() != "default" && getContenido() != "}")
            {
                ListaInstruccionesCase(evaluacion);
            }
        }

        //Instruccion -> Printf | Scanf | If | While | do while | For | Switch | Asignacion
        //Funcion que genera numeros aleatorios

        private void Instruccion(bool evaluacion)
        {
            if (getContenido() == "printf")
            {
                Printf(evaluacion);
            }
            else if (getContenido() == "scanf")
            {
                Scanf(evaluacion);
            }
            else if (getContenido() == "if")
            {
                If(evaluacion);
            }
            else if (getContenido() == "while")
            {
                While(evaluacion);
            }
            else if (getContenido() == "do")
            {
                Do(evaluacion);
            }
            else if (getContenido() == "for")
            {
                For(evaluacion);
            }
            else if (getContenido() == "switch")
            {
                Switch(evaluacion);
            }
            else
            {
                Asignacion(evaluacion);
            }
        }

        private Variable.TipoDato evaluaNumero(float resultado)
        {
            if (resultado % 1 != 0)
            {
                return Variable.TipoDato.Float;
            }
            else if (resultado <= 255)
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
        private void Asignacion(bool evaluacion)
        {
            //Requerimiento 2.- Si no existe la variable levanta la excepcion*

            log.WriteLine();
            log.Write(getContenido() + " = ");
            string nombre = getContenido();

            if (!existeVariable(getContenido()))
                throw new Error("Error : No existe la variable \'" + getContenido() + "\' en linea: " + linea, log);

            match(Tipos.Identificador);
            match(Tipos.Asignacion);
            dominante = Variable.TipoDato.Char;
            Expresion();
            match(";");

            float resultado = stack.Pop();

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
            
            asm.WriteLine("MOV " + nombre + ", AX");

        }

        // While -> while(Condicion) bloque de instrucciones | instruccion
        private void While(bool evaluacion)
        {
            match("while");
            match("(");
            bool validarWhile = Condicion("");
            if (!evaluacion)
            {
                validarWhile = evaluacion;
            }
            // Requerimiento 4
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(validarWhile);
            }
            else
            {
                Instruccion(evaluacion);
            }
        }

        // Do -> do bloque de instrucciones | intruccion while(Condicion)
        private void Do(bool evaluacion)
        {
            match("do");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(evaluacion);
            }
            else
            {
                Instruccion(evaluacion);
            }
            match("while");
            match("(");
            // Requerimiento 4
            bool validarDo = Condicion("");
            if (!evaluacion)
            {
                validarDo = evaluacion;
            }
            match(")");
            match(";");
        }
        // For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
        private void For(bool evaluacion)
        {
            string etquietaInicioFOR = "inicioFor" + cFOR;
            string etquietaFinFOR = "finFor" + cFOR++;
            asm.WriteLine(etquietaInicioFOR + ":");
            match("for");
            match("(");
            Asignacion(evaluacion);
            // Requerimiento 4
            // Requerimiento 6:
            // a) Necesito guardar la posición del archivo del texto

            bool validarFor;

            int posicionGuardada = contador;
            int lineaGuardada = linea;
            string variable = getContenido();
            do
            {
                validarFor = Condicion("");
                if (!evaluacion)
                {
                    validarFor = evaluacion;
                }
                // b) Agregar un ciclo while, después del validarFor() * 

                match(";");
                int sumaresta = IncrementoSintaxis(evaluacion);
                match(")");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(validarFor);
                }
                else
                {
                    Instruccion(validarFor);
                }
                // c) Regresar a la posicion de lectura del archivo
                // d) Sacar otro valor de la pila
                if (validarFor)
                {
                    if(sumaresta == 1)
                    {
                        modVariable(variable, getValor(variable) + 1);
                    }
                    else if(sumaresta == -1)
                    {
                        modVariable(variable, getValor(variable) - 1);
                    }
                    contador = posicionGuardada - variable.Length;
                    linea = lineaGuardada;
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(contador, SeekOrigin.Begin);

                    NextToken();
                }
            }
            while (validarFor);
            asm.WriteLine(etquietaFinFOR + ":");
        }

        //Incremento -> Identificador ++ | --
        private void Incremento(bool evaluacion)
        {
            string variable = getContenido();
            //Requerimiento 2.- Si no existe la variable levanta la excepcion
            if (!existeVariable(getContenido()))
                throw new Error("Error : No existe variable \'" + getContenido() + "\' en linea: " + linea, log);
            NextToken();
            if (getContenido() == "++")
            {
                NextToken();
                if (evaluacion)
                {
                    modVariable(variable, getValor(variable) + 1);
                }
            }
            if (getContenido() == "--")
            {
                NextToken();
                if (evaluacion)
                {
                    modVariable(variable, getValor(variable) - 1);
                }
            }
        }
        private int IncrementoSintaxis(bool evaluacion)
        {
            string variable = getContenido();
            //Requerimiento 2.- Si no existe la variable levanta la excepcion
            if (!existeVariable(getContenido()))
                throw new Error("Error : No existe variable \'" + getContenido() + "\' en linea: " + linea, log);
            match(Tipos.Identificador);
            if (getContenido() == "++")
            {
                match("++");
                return 1;
            }
            if (getContenido() == "--")
            {
                match("--");
                return -1;
            }
            return 0;
        }

        //Switch -> switch (Expresion) {Lista de casos} | (default: )
        private void Switch(bool evaluacion)
        {
            match("switch");
            match("(");
            Expresion();
            stack.Pop();
            match(")");
            match("{");
            ListaDeCasos(evaluacion);
            if (getContenido() == "default")
            {
                match("default");
                match(":");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(evaluacion);
                }
                else
                {
                    Instruccion(evaluacion);
                }
            }
            match("}");
        }

        //ListaDeCasos -> case Expresion: listaInstruccionesCase (break;)? (ListaDeCasos)?
        private void ListaDeCasos(bool evaluacion)
        {
            match("case");
            Expresion();
            stack.Pop();
            match(":");
            ListaInstruccionesCase(evaluacion);
            if (getContenido() == "break")
            {
                match("break");
                match(";");
            }
            if (getContenido() == "case")
            {
                ListaDeCasos(evaluacion);
            }
        }

        //Condicion -> Expresion operador relacional Expresion
        private bool Condicion(string etiqueta)
        {
            Expresion();
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion();
            float e2 = stack.Pop();
            asm.WriteLine("pop BX");
            float e1 = stack.Pop();
            asm.WriteLine("pop AX");
            asm.WriteLine("CMP AX, BX");
            
            switch (operador)
            {
                case "==":
                    asm.WriteLine("JNE " + etiqueta);
                    return e1 == e2;
                case ">":
                    asm.WriteLine("JLE " + etiqueta);
                    return e1 > e2;
                case "<":
                    asm.WriteLine("JGE " + etiqueta);
                    return e1 < e2;
                case ">=":
                    asm.WriteLine("JL " + etiqueta);
                    return e1 >= e2;
                case "<=":
                    asm.WriteLine("JG " + etiqueta);
                    return e1 <= e2;
                default:
                    asm.WriteLine("JE " + etiqueta);
                    return e1 != e2;
            }
        }

        //If -> if(Condicion) bloque de instrucciones (else bloque de instrucciones)?
        private void If(bool evaluacion)
        {
            string etquietaIF = "if" + ++cIF;
            match("if");
            match("(");
            // Requerimiento 4 
            bool validarIf = Condicion(etquietaIF);
            if (!evaluacion)
            {
                validarIf = evaluacion;
            }
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(validarIf);
            }
            else
            {
                Instruccion(validarIf);
            }

            if (getContenido() == "else")
            {
                match("else");
                // Requerimiento 4
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(!validarIf);
                }
                else
                {
                    Instruccion(!validarIf);
                }
            }
            asm.WriteLine(etquietaIF + ":");
        }

        //Printf -> printf(cadena|expresion);
        private void Printf(bool evaluacion)
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
                }
                match(Tipos.Cadena);
            }
            else
            {
                Expresion();
                float resultado = stack.Pop();
                if (evaluacion)
                {
                    Console.Write(stack.Pop());
                }
            }
            match(")");
            match(";");
        }
        //Scanf -> scanf(cadena, &Identificador);
        private void Scanf(bool evaluacion)
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
        private void Expresion()
        {

            Termino();
            MasTermino();
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino();
                log.Write(operador + " ");
                float n1 = stack.Pop();
                float n2 = stack.Pop();
                switch (operador)
                {
                    case "+":
                        stack.Push(n2 + n1);
                        break;

                    case "-":
                        stack.Push(n2 - n1);
                        break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        //PorFactor -> (OperadorFactor Factor)? 
        private void PorFactor()
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor();
                log.Write(operador + " ");
                float n1 = stack.Pop();
                float n2 = stack.Pop();
                switch (operador)
                {
                    case "*":
                        stack.Push(n2 * n1);
                        break;

                    case "/":
                        stack.Push(n2 / n1);
                        break;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                log.Write(getContenido() + " ");
                if (dominante < evaluaNumero(float.Parse(getContenido())))
                {
                    dominante = evaluaNumero(float.Parse(getContenido()));
                }
                stack.Push(float.Parse(getContenido()));
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                if (!existeVariable(getContenido()))
                    throw new Error("Error : No existe variable \'" + getContenido() + "\' en linea: " + linea, log);
                log.Write(getContenido() + " ");
                // Requerimiento 1
                if (dominante < getTipo(getContenido()))
                {
                    dominante = getTipo(getContenido());
                }
                stack.Push(getValor(getContenido()));
                // Requerimiento 3 a)
                // pasamos al siguiente token 

                match(Tipos.Identificador);
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
                Expresion();
                match(")");
                if (huboCasteo)
                {
                    float variable = stack.Pop();
                    dominante = casteo;
                    stack.Push(convert(variable, casteo.ToString()));

                    // Requerimiento 2.- Saco un elemento del stack
                    //                   Convierto ese valor al equialente en casteo
                    // Requerimiento 3.- Ej. Si el casteo es (char) y el pop regresa un 256
                    //                   el valor equivalente en casteo es 0
                }
            }
        }
        ~Lenguaje()
        {
            Console.WriteLine("Destructor");
            cerrar();
        }
    }
}